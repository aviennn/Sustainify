using Sustainify1.Model;
using Sustainify1.ViewModel;
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
    public partial class SustainifyView : ContentPage
    {
        SustainifyViewModel viewModel;
        public SustainifyView()
        {
            InitializeComponent();
            viewModel = new SustainifyViewModel();
        }
        private void sustainifyView()
        {
            var res = viewModel.GetAllSustanifies().Result;
            lstData.ItemsSource = res;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            sustainifyView();
            NavigationPage.SetHasBackButton(this, false);

        }
        private void btnAddRecord(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new SustainifyCreate());
        }
        private async void lsdata_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SustainifyModel obj = (SustainifyModel)e.SelectedItem;
                string res = await DisplayActionSheet("Operation", "Cancel", null, "Update", "Delete");

                switch (res)
                {
                    case "Update":
                        await this.Navigation.PushAsync(new SustainifyCreate(obj));
                        break;

                    case "Delete":
                        viewModel.DeleteAction(obj);
                        sustainifyView();

                      
                        MessagingCenter.Send(this, "DeleteChallenge", obj);
                        break;
                }
                lstData.SelectedItem = null;
            }
        }

    }
}