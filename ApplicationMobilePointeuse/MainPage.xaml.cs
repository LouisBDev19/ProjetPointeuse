namespace ApplicationMobilePointeuse
{
    public partial class MainPage : ContentPage
    {
        private WebAuthenticatorResult authResult;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ConnectionClicked(object sender, EventArgs e)
        {
            try
            {
                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new Uri("https://moodle.3il.fr/"),
                    new Uri("myapp://"));

                string accessToken = authResult?.AccessToken;

                // Do something with the token
            }
            catch (TaskCanceledException ex)
            {
                string accessToken = authResult?.AccessToken;
            }
        }
    }
}