using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Sustainify1.Model;
using Sustainify1.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Sustainify1.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserTabbedPage : TabbedPage
    {
        SustainifyViewModel viewModel;
        private bool isNavigating = false;
        ObservableCollection<SustainifyModel> challenges;
        public ICommand DeleteCommand { get; }

        public UserTabbedPage()
        {
            InitializeComponent();
            viewModel = new SustainifyViewModel();
            challenges = new ObservableCollection<SustainifyModel>();
            LoadSavedChallenges();
            LoadTotalPoints();
            MyChallengesListView.ItemsSource = challenges;

            DeleteCommand = new Command<SustainifyModel>(RemoveChallenge);

            MessagingCenter.Subscribe<ActionDetails>(this, "PointsUpdated", (sender) => {
                LoadTotalPoints();
            });

            MessagingCenter.Subscribe<ActionDetails, SustainifyModel>(this, "AddChallenge", (sender, arg) =>
            {
                AddChallenge(arg);
            });

            MessagingCenter.Subscribe<SustainifyView, SustainifyModel>(this, "DeleteChallenge", (sender, arg) =>
            {
                RemoveChallenge(arg);
            });

            MessagingCenter.Subscribe<ActionDetailsHome, SustainifyModel>(this, "RemoveChallenge", (sender, arg) =>
            {
                RemoveChallenge(arg);
            });

            BindingContext = this;
        }

        private void OnBindingContextChanged(object sender, EventArgs e)
        {
            var grid = sender as Grid;
            var model = grid.BindingContext as SustainifyModel;
            var categoryImage = grid.FindByName<Image>("CategoryImage");

            if (model != null && categoryImage != null)
            {
                switch (model.Category.ToLower())
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
        }

        private void RemoveChallenge(SustainifyModel challenge)
        {
            var itemToRemove = challenges.FirstOrDefault(c => c.ActionCode == challenge.ActionCode);
            if (itemToRemove != null)
            {
                challenges.Remove(itemToRemove);
                SaveChallenges();
            }
        }

        private void LoadSavedChallenges()
        {
            string challengesJson = Preferences.Get("SavedChallenges", string.Empty);
            if (!string.IsNullOrEmpty(challengesJson))
            {
                var challengesList = JsonConvert.DeserializeObject<List<SustainifyModel>>(challengesJson);
                foreach (var challenge in challengesList)
                {
                    challenges.Add(challenge);
                }
            }
        }

        public void AddChallenge(SustainifyModel challenge)
        {
            ResetCheckboxPreferences(challenge.ActionCode, challenge.NumberOfCheckboxes);
            challenges.Add(challenge);
            SaveChallenges();

            MessagingCenter.Send(this, "EnableCheckboxes", challenge.ActionCode);
        }

        private void ResetCheckboxPreferences(string actionCode, int numberOfCheckboxes)
        {
            for (int i = 1; i <= numberOfCheckboxes; i++)
            {
                Preferences.Set($"{actionCode}_chkDay{i}", false);
            }
        }


        private void SaveChallenges()
        {
            var challengesList = challenges.Select(c => new
            {
                c.ActionCode,
                c.Description,
                c.Category,
                c.ImpactLevel,
                c.ImpactLevelDescription,
                c.Frequency,
                c.NumberOfCheckboxes
            }).ToList();

            string challengesJson = JsonConvert.SerializeObject(challengesList);
            Preferences.Set("SavedChallenges", challengesJson);
        }

        private void userTabbedPage()
        {
            var res = viewModel.GetAllSustanifies().Result;
            lstData.ItemsSource = res;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            userTabbedPage();
            NavigationPage.SetHasBackButton(this, false);
            lstData.SelectionChanged += OnItemSelected;
            LoadTotalPoints();
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (isNavigating)
                return;

            if (e.CurrentSelection.Count > 0)
            {
                isNavigating = true;
                var selectedAction = (SustainifyModel)e.CurrentSelection[0];
                await Navigation.PushAsync(new ActionDetails(selectedAction));
                ((CollectionView)sender).SelectedItem = null;
                isNavigating = false;
            }
        }

        private async void OnItemSelectedHome(object sender, SelectionChangedEventArgs e)
        {
            if (isNavigating)
                return;

            if (e.CurrentSelection.Count > 0)
            {
                isNavigating = true;
                var selectedAction = (SustainifyModel)e.CurrentSelection[0];
                await Navigation.PushAsync(new ActionDetailsHome(selectedAction));
                ((CollectionView)sender).SelectedItem = null;
                isNavigating = false;
            }
        }

        private void LoadLevel(int totalPoints)
        {
            Dictionary<int, int> levelThresholds = new Dictionary<int, int>
    {
        { 200, 2 },
        { 400, 3 },
        { 600, 4 },
        { 800, 5 },
        { 1000, 6 },
        { 1200, 7 },
        { 1400, 8 },
        { 1600, 9 },
        { 1800, 10 },
    };

            int level = 1;

            if (totalPoints >= levelThresholds.Keys.Max())
            {
                level = levelThresholds.Values.Max();
            }
            else
            {
                foreach (var threshold in levelThresholds)
                {
                    if (totalPoints >= threshold.Key)
                    {
                        level = threshold.Value;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            lblLevel.Text = level.ToString();

            if (level < levelThresholds.Values.Max())
            {
                int nextLevelThreshold = levelThresholds.FirstOrDefault(t => t.Value == level + 1).Key;
                int pointsToNextLevel = nextLevelThreshold - totalPoints;
                lblPointsToNextLevel.Text = $"{pointsToNextLevel} points to go to reach level {level + 1}.";
            }
            else
            {
                lblPointsToNextLevel.Text = "Congratulations! You have reached the maximum level!";
            }
        }

        private void LoadTotalPoints()
        {
            int totalPoints = Preferences.Get("TotalPoints", 0);
            lblTotalPoints.Text = totalPoints.ToString();

            LoadLevel(totalPoints);
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
