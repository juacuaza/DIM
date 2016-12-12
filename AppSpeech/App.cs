using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace AppSpeech
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

            private System.Speech.Recognition.SpeechRecognitionEngine _recognizer =
               new SpeechRecognitionEngine();
            private SpeechSynthesizer synth = new SpeechSynthesizer();

            private void Form1_Load(object sender, EventArgs e)
            {
                synth.Speak("Inicializando la Aplicación");

                Grammar grammar = CreateGrammarBuilderRGBSemantics2(null);
                _recognizer.SetInputToDefaultAudioDevice();
                _recognizer.UnloadAllGrammars();
                _recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 50);
                grammar.Enabled = true;
                _recognizer.LoadGrammar(grammar);
                _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);
                //reconocimiento asíncrono y múltiples veces
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                synth.Speak("Aplicación preparada para reconocer su voz");
            }



            void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
            {
                RecognizedWordUnit strOpcion;

                //obtenemos un diccionario con los elementos semánticos
                SemanticValue semantics = e.Result.Semantics;

                strOpcion = e.Result.Words[e.Result.Words.Count - 1];
                string rawText = e.Result.Text;
                RecognitionResult result = e.Result;

                switch (strOpcion.Text.ToLower())
                {
                    case "verde":
                    case "azul":
                    case "rojo":
                        this.label1.Text = rawText;
                        this.BackColor = Color.FromArgb((int)semantics["rgb"].Value);
                        Update();
                        synth.Speak(rawText);
                        break;

                    case "letra":
                        float currentSize = this.label1.Font.Size;
                        currentSize += 2.0F;
                        this.label1.Font = new Font(this.label1.Font.Name, currentSize, this.label1.Font.Style, label1.Font.Unit);
                        this.label1.Text = rawText;
                        Update();
                        synth.Speak(rawText);
                        break;

                    default:
                        this.label1.Text = "No info provided.";
                        break;

                }
            }


            private Grammar CreateGrammarBuilderRGBSemantics2(params int[] info)
            {
                synth.Speak("Creando ahora la gramática");
                Choices colorChoice = new Choices();
                Choices letraChoice = new Choices();

                // foreach (string colorName in System.Enum.GetNames(typeof(KnownColor)))
                // {
                //     SemanticResultValue choiceResultValue =
                //         new SemanticResultValue(colorName, Color.FromName(colorName).ToArgb());
                //     GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
                //     colorChoice.Add(resultValueBuilder);
                // }

                SemanticResultValue choiceResultValue =
                        new SemanticResultValue("Rojo", Color.FromName("Red").ToArgb());
                GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
                colorChoice.Add(resultValueBuilder);

                choiceResultValue =
                       new SemanticResultValue("Azul", Color.FromName("Blue").ToArgb());
                resultValueBuilder = new GrammarBuilder(choiceResultValue);
                colorChoice.Add(resultValueBuilder);

                choiceResultValue =
                       new SemanticResultValue("Verde", Color.FromName("Green").ToArgb());
                resultValueBuilder = new GrammarBuilder(choiceResultValue);
                colorChoice.Add(resultValueBuilder);

                SemanticResultKey choiceResultKey = new SemanticResultKey("rgb", colorChoice);
                GrammarBuilder colores = new GrammarBuilder(choiceResultKey);

                choiceResultValue =
                        new SemanticResultValue("Letra", this.label1.Size.ToString());
                resultValueBuilder = new GrammarBuilder(choiceResultValue);
                letraChoice.Add(resultValueBuilder);



                //Choices dos_al = new Choices(colorChoice, letraChoice);



                choiceResultKey = new SemanticResultKey("letra", letraChoice);
                GrammarBuilder tletra = new GrammarBuilder(choiceResultKey);

                GrammarBuilder poner = "Poner";
                GrammarBuilder cambiar = "Cambiar";
                GrammarBuilder fondo = "Fondo";
                GrammarBuilder tam = "Tamaño";

                Choices PoCa = new Choices(poner, cambiar);
                GrammarBuilder frase1 = new GrammarBuilder(PoCa);

                Choices FoTa = new Choices(fondo, tam);
                GrammarBuilder frase2 = new GrammarBuilder(FoTa);

                frase1.Append(frase2);
                Choices col_tam = new Choices(colores, tletra);
                GrammarBuilder frase3 = new GrammarBuilder(col_tam);
                frase1.Append(frase3);

                try
                {
                    Grammar grammar = new Grammar(frase1);
                    grammar.Name = "Poner/Cambiar Fondo/Tamaño";
                    return grammar;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                    return null;
                }


            }
        }
    }