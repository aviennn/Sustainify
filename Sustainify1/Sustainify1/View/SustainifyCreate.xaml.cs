using Sustainify1.ViewModel;
using Sustainify1.Model;
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
    public partial class SustainifyCreate : ContentPage
    {
        SustainifyViewModel _viewModel;
        bool _isUpdate;
        int actionID;
        public SustainifyCreate()
        {
            InitializeComponent();
            _viewModel = new SustainifyViewModel();
            _isUpdate = false;
        }
        public SustainifyCreate(SustainifyModel obj)
        {
            InitializeComponent();
            _viewModel = new SustainifyViewModel();
            if (obj != null)
            {
                actionID = obj.Id;
                actionCodeLabel.Text = obj.ActionCode;
                actionDescription.Text = obj.Description;
                categoryPicker.SelectedItem = obj.Category;
                impactLevelPicker.SelectedItem = obj.ImpactLevel;
                actionImpactLevelDescription.Text = obj.ImpactLevelDescription;
                frequencyPicker.SelectedItem = obj.Frequency;
                numberOfCheckboxes.Text = obj.NumberOfCheckboxes.ToString();


                _isUpdate |= true;
            }
        }

        private async void btnSaveUpdate_Clicked(object sender, EventArgs e)
        {
            if (!int.TryParse(numberOfCheckboxes.Text, out int numberOfCheckboxesValue))
            {
                await DisplayAlert("Error", "Please enter a valid number of checkboxes", "OK");
                return;
            }

            SustainifyModel obj = new SustainifyModel
            {
                ActionCode = actionCodeLabel.Text,
                Description = actionDescription.Text,
                Category = categoryPicker.SelectedItem?.ToString(),
                ImpactLevel = impactLevelPicker.SelectedItem?.ToString(),
                ImpactLevelDescription = actionImpactLevelDescription.Text,
                Frequency = frequencyPicker.SelectedItem?.ToString(),
                NumberOfCheckboxes = numberOfCheckboxesValue 
            };

            if (_isUpdate)
            {
                obj.Id = actionID;
                await _viewModel.UpdateAction(obj);
            }
            else
            {
                _viewModel.InsertAction(obj);
            }

            await Navigation.PopAsync();
        }
    }
}