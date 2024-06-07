using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sustainify1.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingPage : ContentPage
    {
        public LandingPage()
        {
            InitializeComponent();
        }
        private void btnLogin(object sender, EventArgs e)
        {
            if (txtUsername.Text == "admin" && txtPassword.Text == "admin123")
            {
                Navigation.PushAsync(new SustainifyView());
            }
            else if (txtUsername.Text == "user" && txtPassword.Text == "user123")
            {
                Navigation.PushAsync(new UserTabbedPage());
            }
            else
            {
                DisplayAlert("Please re-enter right credentials", "Username or Password is Incorrect", "Ok");
            }

        }
    }
}