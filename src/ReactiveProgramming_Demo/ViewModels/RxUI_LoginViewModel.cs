using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace ReactiveProgramming_Demo.ViewModels
{
    public class RxUI_LoginViewModel : ReactiveObject
    {
        private readonly ReactiveCommand<Unit, bool> loginCommand;
        private string userName;
        private string password;
        private bool isRunning;

        private ObservableAsPropertyHelper<bool> _isRunning2;

        public bool IsRunning2 => _isRunning2.Value;

        public ReactiveCommand<Unit, bool> LoginCommand => this.loginCommand;

        public string UserName
        {
            get { return this.userName; }
            set { this.RaiseAndSetIfChanged(ref this.userName, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { this.RaiseAndSetIfChanged(ref this.password, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.RaiseAndSetIfChanged(ref this.isRunning, value); }
        }

        public RxUI_LoginViewModel()
        {
            var canLogin = this.WhenAnyValue(
            x => x.UserName,
            x => x.Password,
            (userName, password) => !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password));

            loginCommand = ReactiveCommand.CreateFromObservable(LoginAsync, canLogin);

            loginCommand.Subscribe(isLoginSuccess =>
            {
                if(isLoginSuccess)
                    new MessageDialog("Succes! You are logged in").ShowAsync();
                else
                    new MessageDialog("Incorrect password!").ShowAsync();
            });

            loginCommand.IsExecuting.Subscribe(running =>
            {
                IsRunning = running;
            });

            _isRunning2 = loginCommand.IsExecuting.ToProperty(this, vm => vm.IsRunning2, out _isRunning2);
        }

        private IObservable<bool> LoginAsync()
        {
            return Observable
                .FromAsync(() => DoLoginAsync(userName, password))
                .Do(result =>
                {
                    if (result)
                        Debug.WriteLine("hello");
                    else
                        Debug.WriteLine("bye");
                });
        }

        private async Task<bool> DoLoginAsync(string userName, string password)
        {
            await Task.Delay(2000);
            return password == "rx is cool";
        }
    }
}
