﻿using Nest;
using NuSearch.Domain.Data;
using NuSearch.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using NuSearch.Domain;
using NuSearch.Domain.Extensions;
using ShellProgressBar;

namespace NuSearch.Indexer
{
	class Program
	{
		private static ElasticClient Client { get; set; }
		private static NugetDumpReader DumpReader { get; set; }
		private static string CurrentIndexName { get; set; }

		static void Main(string[] args)
		{
			Client = NuSearchConfiguration.GetClient();
			CurrentIndexName = NuSearchConfiguration.CreateIndexName();

			DeleteIndexIfExists();

			DumpReader = new NugetDumpReader(@"C:\Projects\test\elasticsearch-net-example\nuget-data-dec-2016");
			CreateIndex();
			IndexDumps();
			SwapAlias();

			Console.Read();
		}

		static void CreateIndex()
		{
			Client.CreateIndex(CurrentIndexName, i => i
				.Settings(s => s
					.NumberOfShards(2)
					.NumberOfReplicas(0)
					.Analysis(analysis => analysis
					.Tokenizers(tokenizers => tokenizers
						.Pattern("nuget-id-tokenizer", p => p.Pattern(@"\W+"))
					)
					.TokenFilters(tokenfilters => tokenfilters
						.WordDelimiter("nuget-id-words", w => w
							.SplitOnCaseChange()
							.PreserveOriginal()
							.SplitOnNumerics()
							.GenerateNumberParts(false)
							.GenerateWordParts()
						)
					)
					.Analyzers(analyzers => analyzers
						.Custom("nuget-id-analyzer", c => c
							.Tokenizer("nuget-id-tokenizer")
							.Filters("nuget-id-words", "lowercase")
						)
						.Custom("nuget-id-keyword", c => c
							.Tokenizer("keyword")
							.Filters("lowercase")
						)
					)
				)
				)
				.Mappings(m => m
					.Map<Package>(map => map
						.AutoMap()
						.Properties(ps => ps
							.Text(s => s
								.Name(p => p.Id)
								.Analyzer("nuget-id-analyzer")
								.Fields(f => f
									.Text(p => p.Name("keyword").Analyzer("nuget-id-keyword"))
									.Keyword(p => p.Name("raw"))
								)
							)
							.Completion(c => c
								.Name(p => p.Suggest)
							)
							.Nested<PackageVersion>(n => n
								.Name(p => p.Versions.First())
								.AutoMap()
								.Properties(pps => pps
									.Nested<PackageDependency>(nn => nn
										.Name(pv => pv.Dependencies.First())
										.AutoMap()
									)
								)
							)
							.Nested<PackageAuthor>(n => n
								.Name(p => p.Authors.First())
								.AutoMap()
								.Properties(pps => pps
									.Text(s => s
										.Name(a => a.Name)
										.Fields(fs => fs
											.Keyword(ss => ss
												.Name("raw")
											)
										)
									)
								)
							)
						)
					)
				)
			);
		}

		private static void SwapAlias()
		{
			var indexExists = Client.IndexExists(NuSearchConfiguration.LiveIndexAlias).Exists;

			Client.Alias(aliases =>
			{
				if (indexExists)
					aliases.Add(a => a.Alias(NuSearchConfiguration.OldIndexAlias).Index(NuSearchConfiguration.LiveIndexAlias));

				return aliases
					.Remove(a => a.Alias(NuSearchConfiguration.LiveIndexAlias).Index("*"))
					.Add(a => a.Alias(NuSearchConfiguration.LiveIndexAlias).Index(CurrentIndexName));
			});

			var oldIndices = Client.GetIndicesPointingToAlias(NuSearchConfiguration.OldIndexAlias)
				.OrderByDescending(name => name)
				.Skip(2);

			foreach (var oldIndex in oldIndices)
				Client.DeleteIndex(oldIndex);
		}

		static void DeleteIndexIfExists()
		{
			if (Client.IndexExists("nusearch").Exists)
				Client.DeleteIndex("nusearch");
		}

		static void IndexDumps()
		{
			Console.WriteLine("Setting up a lazy xml files reader that yields packages...");
			var packages = DumpReader.GetPackages().Take(2000);

			Console.WriteLine("Indexing documents into elasticsearch...");
			var waitHandle = new CountdownEvent(1);

			var bulkAll = Client.BulkAll(packages, b => b
				.Index(CurrentIndexName)
				.BackOffRetries(2)
				.BackOffTime("30s")
				.RefreshOnCompleted(true)
				.MaxDegreeOfParallelism(4)
				.Size(1000)
			);

			bulkAll.Subscribe(new BulkAllObserver(
				onNext: (b) => { Console.Write("."); },
				onError: (e) => { throw e; },
				onCompleted: () => waitHandle.Signal()
			));
			waitHandle.Wait();
			Console.WriteLine("Done.");
		}
	}
}
