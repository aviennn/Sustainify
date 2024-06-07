using Sustainify1.Model;
using System.Linq;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sustainify1.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActionDetailsHome : ContentPage
    {
        private int completedCount = 0;
        private int totalCheckboxes = 0;
        private int timesCompleted = 0;
        private string actionCode;

        public ActionDetailsHome(SustainifyModel action)
        {
            InitializeComponent();
            BindingContext = action;
            actionCode = action.ActionCode;
            CreateCheckboxes(action.NumberOfCheckboxes);
            LoadTimesCompleted();
            UpdateTimesCompletedLabel();
            DisplayPoints(action.NumberOfCheckboxes);
            SetCategoryImage(action.Category); 

        }

        private void SetCategoryImage(string category)
        {
            switch (category.ToLower())
            {
                case "water":
                    categoryImage.Source = "water.png";
                    break;
                case "climate":
                    categoryImage.Source = "climate.png";
                    break;
                case "waste":
                    categoryImage.Source = "waste.png";
                    break;
                case "energy":
                    categoryImage.Source = "energy.png";
                    break;
                case "biodiversity":
                    categoryImage.Source = "biodiversity.png";
                    break;
                case "agriculture":
                    categoryImage.Source = "agriculture.png";
                    break;
                case "air":
                    categoryImage.Source = "air.png";
                    break;
                default:
                    categoryImage.Source = "default.png";
                    break;
            }
        }

        private void CreateCheckboxes(int numberOfCheckboxes)
        {
            totalCheckboxes = numberOfCheckboxes;

            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                var checkBox = new CheckBox
                {
                    ClassId = $"{actionCode}_chkDay{i}",
                    IsChecked = Preferences.Get($"{actionCode}_chkDay{i}", false),
                    IsEnabled = true 
                };
                checkBox.CheckedChanged += OnCheckboxChecked;

                var label = new Label
                {
                    Text = i.ToString(),
                    VerticalOptions = LayoutOptions.Center
                };

                var horizontalLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };
                horizontalLayout.Children.Add(checkBox);
                horizontalLayout.Children.Add(label);

                CheckboxContainer.Children.Add(horizontalLayout);

                if (checkBox.IsChecked)
                    completedCount++;
            }

            if (completedCount == totalCheckboxes)
            {
                lblChallengeCompleted.IsVisible = true;
                lblChallengeCompleted.Text = "Challenge Completed!";
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            for (int i = 1; i <= totalCheckboxes; i++)
            {
                var checkboxId = $"{actionCode}_chkDay{i}";
                var checkBox = CheckboxContainer.Children
                    .SelectMany(child => ((StackLayout)child).Children)
                    .OfType<CheckBox>()
                    .FirstOrDefault(c => c.ClassId == checkboxId);

                if (checkBox != null)
                {
                    checkBox.IsChecked = Preferences.Get(checkboxId, false);
                }
            }
        }

        private void OnCheckboxChecked(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;

            if (e.Value)
                completedCount++;
            else
                completedCount--;

            Preferences.Set(checkBox.ClassId, e.Value);

            if (completedCount == totalCheckboxes)
            {
                lblChallengeCompleted.IsVisible = true;
                lblChallengeCompleted.Text = "Challenge Completed!";
                IncrementTimesCompleted();
                SaveTimesCompleted();
                UpdateTimesCompletedLabel();
                AddPoints();
                MessagingCenter.Send(this, "RemoveChallenge", (SustainifyModel)BindingContext);

               
                ResetCheckboxes();

                Navigation.PopAsync();
            }
            else
            {
                lblChallengeCompleted.IsVisible = false;
            }
        }

        private void ResetCheckboxes()
        {
            foreach (var child in CheckboxContainer.Children)
            {
                if (child is StackLayout stackLayout)
                {
                    var checkBox = stackLayout.Children.OfType<CheckBox>().FirstOrDefault();
                    if (checkBox != null)
                    {
                        checkBox.CheckedChanged -= OnCheckboxChecked;
                        checkBox.IsChecked = false;
                        Preferences.Set(checkBox.ClassId, false);
                        checkBox.CheckedChanged += OnCheckboxChecked;
                    }
                }
            }

            completedCount = 0;
            lblChallengeCompleted.IsVisible = false;
        }

        private void IncrementTimesCompleted()
        {
            timesCompleted++;
            Preferences.Set($"{actionCode}_TimesCompleted", timesCompleted);
        }

        private void SaveTimesCompleted()
        {
            Preferences.Set($"{actionCode}_TimesCompleted", timesCompleted);
        }

        private void LoadTimesCompleted()
        {
            timesCompleted = Preferences.Get($"{actionCode}_TimesCompleted", 0);
        }

        private void UpdateTimesCompletedLabel()
        {
            lblTimesCompleted.Text = $"Times Completed: {timesCompleted}";
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            foreach (var child in CheckboxContainer.Children)
            {
                if (child is StackLayout stackLayout)
                {
                    var checkBox = stackLayout.Children.OfType<CheckBox>().FirstOrDefault();
                    if (checkBox != null)
                    {
                        checkBox.CheckedChanged -= OnCheckboxChecked;
                        checkBox.IsChecked = false;
                        checkBox.CheckedChanged += OnCheckboxChecked;
                        Preferences.Set(checkBox.ClassId, false);
                    }
                }
            }

            completedCount = 0;
            lblChallengeCompleted.IsVisible = false;
            SaveTimesCompleted();
            UpdateTimesCompletedLabel();
        }

        private void AddPoints()
        {
            int points = 0;

            if (totalCheckboxes >= 1 && totalCheckboxes <= 3)
            {
                points = 20;
            }
            else if (totalCheckboxes >= 4 && totalCheckboxes <= 6)
            {
                points = 25;
            }
            else if (totalCheckboxes >= 7 && totalCheckboxes <= 10)
            {
                points = 30;
            }

            int totalPoints = Preferences.Get("TotalPoints", 0) + points;
            Preferences.Set("TotalPoints", totalPoints);

            MessagingCenter.Send(this, "PointsUpdated");
        }

        private void DisplayPoints(int numberOfCheckboxes)
        {
            int points = 0;

            if (numberOfCheckboxes >= 1 && numberOfCheckboxes <= 3)
            {
                points = 20;
            }
            else if (numberOfCheckboxes >= 4 && numberOfCheckboxes <= 6)
            {
                points = 25;
            }
            else if (numberOfCheckboxes >= 7 && numberOfCheckboxes <= 10)
            {
                points = 30;
            }

            lblPoints.Text = $"Points can be obtained: {points}";
        }
    }
}
