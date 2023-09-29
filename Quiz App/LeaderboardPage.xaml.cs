using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace Quiz_App
{
    /// <summary>
    /// Interaction logic for LeaderboardPage.xaml
    /// </summary>
    
    public partial class LeaderboardPage : Page
    {
        int totalWidth;

        List<string> PlacementColours = new List<string>
        {
            "#E1AD21",
            "#A3A3A3",
            "#A9550F"
        };

        public LeaderboardPage()
        {
            InitializeComponent();
        }

        private void UponScreenInit_Loaded(object sender, RoutedEventArgs e)
        {

            Border AddJSONtoLeaderboard(List<User> FileData, string FilePath)
            {
                Border LeaderboardFrameBorder = new Border();
                LeaderboardFrameBorder.Style = (Style)Resources["LeaderboardFrameBorderStyle"];
                StackPanel leaderboard = new StackPanel();

                leaderboard.Style = (Style)Resources["LeaderboardFrameStyle"];

                LeaderboardFrameBorder.Child = leaderboard;

                string leaderboardName = System.IO.Path.GetFileNameWithoutExtension(FilePath);
                leaderboardName = leaderboardName.Remove(0, 6); // removes the Leader from the name to get the name of the quiz

                TextBlock leaderboardTitle = new TextBlock();
                leaderboardTitle.Text = leaderboardName;
                leaderboardTitle.Style = (Style)Resources["LeaderboardFrameTitleStyle"];

                Border LeaderboardTitleBorder = new Border();
                LeaderboardTitleBorder.Style = (Style)Resources["LeaderboardTitleBorderStyle"];
                LeaderboardTitleBorder.Child = leaderboardTitle;

                leaderboard.Children.Add(LeaderboardTitleBorder);

                for (int index = 0; index <= 9; index++)
                {
                    Border UserScoreTextBoxBorder = new Border();
                    UserScoreTextBoxBorder.Style = (Style)Resources["LeaderboardUserBorderStyle"];
                    
                    DockPanel UserDataTextHolder = new DockPanel();
                    UserDataTextHolder.Style = (Style)Resources["DockPanelStyle"];

                    if (index < 3)
                    {
                        Color color = (Color)ColorConverter.ConvertFromString(PlacementColours[index]);
                        UserScoreTextBoxBorder.Background = new SolidColorBrush(color);
                    }

                    if (index < FileData.Count)
                    {
                        User userData = FileData[index];

                        TextBlock PlaceNumber = new TextBlock();
                        TextBlock Username = new TextBlock();
                        TextBlock Score = new TextBlock();

                        PlaceNumber.Text = $"{index + 1}.";
                        Username.Text = Convert.ToString(userData.name);
                        Score.Text = Convert.ToString(userData.score);

                        PlaceNumber.Style = (Style)Resources["PlaceNameStyle"];
                        Username.Style = (Style)Resources["PlaceNameStyle"];
                        Score.Style = (Style)Resources["ScoreStyle"];

                        UserDataTextHolder.Children.Add(PlaceNumber);
                        UserDataTextHolder.Children.Add(Username);
                        UserDataTextHolder.Children.Add(Score);
                    }
                   
                    UserScoreTextBoxBorder.Child = UserDataTextHolder;

                    leaderboard.Children.Add(UserScoreTextBoxBorder);
                }

                return LeaderboardFrameBorder;
            }

            List<string>? FilePaths = ((App)Application.Current).GlobalLeaderPaths;

            List<Border> LeaderboardFrames = new List<Border>();
           

            foreach (string FilePath in FilePaths)
            {
                string content = File.ReadAllText(FilePath);
                dynamic JSONcontent = JsonSerializer.Deserialize<List<User>>(content);
                Border NewLeaderboard = AddJSONtoLeaderboard(JSONcontent, FilePath);
                LeaderboardFrames.Add(NewLeaderboard);
            }

            totalWidth = 310 * FilePaths.Count;

            ScrollViewStackPanel.Width = totalWidth;
           
            foreach (Border LeaderboardFrame in LeaderboardFrames)
            {
                ScrollViewStackPanel.Children.Add(LeaderboardFrame);
            }
        }   

        void LeftBtn_Click(object sender, RoutedEventArgs e)
        {
            double horizontalOffset = LeaderboardScrollFrame.HorizontalOffset - 380;
            if (horizontalOffset < 0)
            {
                horizontalOffset = 0;
            }

            LeaderboardScrollFrame.ScrollToHorizontalOffset(horizontalOffset);
        }

        void RightBtn_Click(object sender, RoutedEventArgs e)
        {
            double horizontalOffset = LeaderboardScrollFrame.HorizontalOffset + 380;
            if (horizontalOffset > totalWidth)
            {
                horizontalOffset = totalWidth;
            }

            LeaderboardScrollFrame.ScrollToHorizontalOffset(horizontalOffset);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}