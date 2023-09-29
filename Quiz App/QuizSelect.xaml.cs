using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Text.Json;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Quiz_App
{
    /// <summary>
    /// Interaction logic for QuizSelect.xaml
    /// </summary>
    /// 
    public partial class QuizSelect : Page
    {
        public QuizSelect()
        {
            InitializeComponent();
            QuizSelect_Loaded(); // not the bulit-in function, a custom one. if bulit in one is found, copy the code from this one into it!
        }

        private void QuizSelect_Loaded() // just loads the quizes from the JSON files, and makes the buttons for the GUI.
        {

            List<Quiz> Quizzes = ((App)Application.Current).GlobalQuizzes; // this gets the global quiz variable made in MainWindow.xaml.cs

            foreach (Quiz quiz in Quizzes) //makes button for each quiz loaded
            {
                Button QuizClick = new Button();
                Border QuizClickBorder = new Border();

                QuizClick.Content = quiz.QuizName;
                QuizClick.Click += QuizClick_click;
                QuizClick.Style = (Style)Resources["QuizGameBtnStyle"];

                QuizClickBorder.Child = QuizClick;
                QuizClickBorder.Style = (Style)Resources["QuizBtnBorderStyle"];

                ButtonsPanel.Children.Add(QuizClickBorder);
            }
        }


        //Below two are only related to the GUI and Buttons
        private static Quiz? GetQuiz(string name) // finds the quiz object based on its name
        {
            foreach (Quiz q in ((App)Application.Current).GlobalQuizzes)
            {
                if (q.QuizName == name)
                {
                    return q;
                }
            }
           return default;
        }

        private void QuizClick_click(object sender, RoutedEventArgs e) // the click function attributed to each btn
        {
            Button clickedButton = (Button)sender; // get btn
            string quizName = clickedButton.Content.ToString(); // get the content (quiz name) of the btn
            Quiz game = GetQuiz(quizName); // finds the quiz from the list of all of them, to be parsed into the next page

            this.NavigationService.Navigate(new QuizGame(quizName, game)); // loads the game page, parsing the name of the quiz (pure GUI) and the game (core functionaility)
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Page1());
        }
    }
}