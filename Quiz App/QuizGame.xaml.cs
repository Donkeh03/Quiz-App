using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quiz_App
{
    /// <summary>
    /// Interaction logic for QuizGame.xaml
    /// </summary>
    public partial class QuizGame : Page
    {

        //external class-level variables, needed for iterating the the questions, scoring and answering logic
        private string correctAnswer;
        private int score;
        private int currentQuestionIndex;
        private bool running;
        public object Data { get; set; } // very important, holds the quiz needed and the name of the quiz

        public QuizGame(string QuizName, Quiz Game)
        {
            InitializeComponent();
            Data = new { QuizName, Game }; // with the parsed data from the QuizSelect.xaml.cs script, it is put in the data object os it can be used within this script
            StartGame(); // starts the game
        }

        private void StartGame()
        {
            var data = (dynamic)Data; // gets data
            QuizTitle.Content = data.QuizName; // sets the title of the page
            score = 0; // sets the score to 0
            currentQuestionIndex = 0; // begins the counter for indexing the questions
            running = true; // represnts if the game is running or not

            ShowNextQuestion(); // shows the next question
        }

        private void ShowNextQuestion() // shows a question on the screen
        {
            
            var data = (dynamic)Data; // opens data object
            if (currentQuestionIndex < data.Game.QuizQuestions.Count) // iterating over the QuizQuestions
            {
                Question question = data.Game.QuizQuestions[currentQuestionIndex]; // question we are on
                QuestionLbl.Content = question.QuestionText; // effectivly asking the user the question
                string[] answers = question.Answers.Split(":"); // splits the answers string up into the indivual parts, and puts them into a list

                // set the anwser buttons to the corrosponding potential answser
                Btn1Text.Text = answers[0];
                Btn2Text.Text = answers[1];
                Btn3Text.Text = answers[2];
                Btn4Text.Text = answers[3];

                // sets the anwser index (number we need, between 1 and 4)
                correctAnswer = question.CorrectAnswerIndex;
                
                //loop to the next question
                currentQuestionIndex++;
            }
            else // only fired if we have reached the last question
            {
                running = false;
                int QuizLength = data.Game.QuizQuestions.Count;
                this.NavigationService.Navigate(new Results(data.QuizName,score,QuizLength));
            
            }
        }

        private void Respond(object sender, RoutedEventArgs e) // function is attributed to each answer btn on the main screen
        {
            Button clickedButton = (Button)sender; // getting the button
            string option = clickedButton.Name[^1].ToString(); // as the button is name "btn1" this gets the last item so "1". this means we clicked the first btn

            if (option == correctAnswer && running == true) //did user get it right
            {
                score++; // increase score by 1
            }
            ShowNextQuestion(); // go to the next question
        }
    }
}
