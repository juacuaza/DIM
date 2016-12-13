using System;
using System.Drawing;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace AppSpeech
{
    public partial class App : Form
    {
        #region Atributos

        private System.Speech.Recognition.SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        private SpeechSynthesizer synth = new SpeechSynthesizer();
        private Timer timer;
        private int iSegundos;

        #endregion

        #region Constructor

        public App()
        {
            InitializeComponent();
        }

        #endregion

        #region Metodos

        private void Form1_Load(object sender, EventArgs e)
        {
            synth.Speak("Inicializando la Aplicación");

            Grammar grammar = CreateGrammarBuilderSemantics(null);
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
                    this.LBL_Texto.Text = rawText;
                    this.BackColor = Color.FromArgb((int)semantics["rgb"].Value);
                    Update();
                    synth.Speak(rawText);
                    break;

                case "letra":
                    float currentSize = this.LBL_Texto.Font.Size;
                    currentSize += 2.0F;
                    this.LBL_Texto.Font = new Font(this.LBL_Texto.Font.Name, currentSize, this.LBL_Texto.Font.Style, LBL_Texto.Font.Unit);
                    this.LBL_Texto.Text = rawText;
                    Update();
                    synth.Speak(rawText);
                    break;

                case "llamas":
                    this.LBL_Texto.Text = "Mi nombre es " + semantics["llamas"].Value;
                    Update();
                    synth.Speak("Mi nombre es " + semantics["llamas"].Value);
                    break;

                case "salir":
                    this.LBL_Texto.Text = semantics["salir"].Value.ToString();
                    Update();
                    synth.Speak("Hasta la próxima");
                    this.Close();
                    break;

                case "alarma":
                    if (rawText.ToLower().Contains("poner"))
                    {
                        this.LBL_Texto.Text = "La alarma sonará en " + semantics["alarma"].Value.ToString() + " segundos";
                        Update();
                        synth.Speak(this.LBL_Texto.Text);
                        PonerAlarma((int)semantics["alarma"].Value);
                    }
                    else if(rawText.ToLower().Contains("quitar"))
                    {                        
                        if (this.timer != null)
                        {
                            this.timer.Stop();
                            this.BackColor = Color.FromArgb(255, 240, 240, 240);
                            this.LBL_Texto.Text = "Alarma quitada";
                            Update();
                            synth.Speak(this.LBL_Texto.Text);
                        }
                        
                    }
                    break;

                default:
                    this.LBL_Texto.Text = "No info provided.";
                    break;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iSegundos"></param>
        private void PonerAlarma(int iSegundos)
        {
            this.iSegundos = iSegundos;

            timer = new Timer();
            timer.Tick += new System.EventHandler(this.timer_Tick);
            timer.Interval = 1000;
            timer.Start();         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            iSegundos--;
            if (iSegundos >= 0)
            {
                this.LBL_Texto.Text = iSegundos.ToString();
            }
            else
            {
                if (this.BackColor == Color.FromArgb(255, 240, 240, 240))
                {
                    this.BackColor = Color.Red;
                    this.LBL_Texto.Text = "ALARMA";
                }
                else
                {
                    this.BackColor = Color.FromArgb(255, 240, 240, 240);
                    this.LBL_Texto.Text = "";
                }
                //falta poner audio con alarma
            }
                //timer.Stop();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Grammar CreateGrammarBuilderSemantics(params int[] info)
        {
            synth.Speak("Creando ahora la gramática");

            GrammarBuilder comoTe= "Como te";
            GrammarBuilder poner = "Poner";
            GrammarBuilder cambiar = "Cambiar";
            GrammarBuilder fondo = "Fondo";
            GrammarBuilder tam = "Tamaño";
            GrammarBuilder quitar = "Quitar";


            //Regla 0 "Salir"
            Choices cerrarChoices = CreateChoiceSalir();

            //Regla 1 "¿Como te llamas?"
            Choices nombreChoice = CreateChoicesNombre();

            //Regla 2 "Poner alarma"
            Choices alarmaChoice = CreateChoicesAlarma();

            ///se crean el choice de los colores
            Choices colorChoice = CreateChoiceColor();

            ///Se crea el choice de el tamaño letra
            Choices letraChoice = CreateChoiceLetra();



            ///Crea grammar de colores            
            GrammarBuilder gColores = CreateGrammarKey("rgb", colorChoice);

            ///Crea el grammar de letra            
            GrammarBuilder tLetra = CreateGrammarKey("letra", letraChoice);

            ///Crear el grammar de salir
            GrammarBuilder gSalir = CreateGrammarKey("salir", cerrarChoices);

            ///Crea grammar del nombre 
            GrammarBuilder gNombre = CreateGrammarKey("llamas", nombreChoice);

            //crea Grammar de la alarma
            GrammarBuilder galarma = CreateGrammarKey("alarma", alarmaChoice);


            Choices cnombre = new Choices(comoTe);
            GrammarBuilder fNombre = new GrammarBuilder(cnombre);
            fNombre.Append(gNombre);


            Choices calarma = new Choices(poner, quitar);
            GrammarBuilder fPalarma = new GrammarBuilder(calarma);
            fPalarma.Append(galarma);

            Choices PoCa = new Choices(poner, cambiar);
            GrammarBuilder frase1 = new GrammarBuilder(PoCa);

            Choices FoTa = new Choices(fondo, tam);
            GrammarBuilder frase2 = new GrammarBuilder(FoTa);

            frase1.Append(frase2);
            Choices col_tam = new Choices(gColores, tLetra);
            GrammarBuilder frase3 = new GrammarBuilder(col_tam);
            frase1.Append(frase3);


            Choices cFinal = new Choices(fNombre, frase1, gSalir, fPalarma);
            GrammarBuilder fFinal = new GrammarBuilder(cFinal);

            try
            {
                //Grammar grammar = new Grammar(frase1);
                Grammar grammar = new Grammar(fFinal);
                grammar.Name = "Poner/Cambiar Fondo/Tamaño";
                return grammar;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                return null;
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Choices CreateChoiceSalir()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("Salir", "Adios");
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Choices CreateChoicesAlarma()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("Alarma", 30);
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
        }

        /// <summary>
        /// crea los choices de los colores
        /// </summary>
        /// <returns>ColorChoices</returns>
        private Choices CreateChoiceColor()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("Rojo", Color.FromName("Red").ToArgb());
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Azul", Color.FromName("Blue").ToArgb());
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Verde", Color.FromName("Green").ToArgb());
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
        }

        /// <summary>
        /// Crea el choice del tamaño letra
        /// </summary>
        /// <returns>Choice Tamaño letra</returns>
        private Choices CreateChoiceLetra()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("Letra", this.LBL_Texto.Size.ToString());
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
        }

        /// <summary>
        /// Crea el choice del nombre
        /// </summary>
        /// <returns>Choice nombre</returns>
        private Choices CreateChoicesNombre()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("LLamas", "DIMA");
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
        }

        /// <summary>
        /// Crea el Key de la gramatica
        /// </summary>
        /// <param name="strFrase">string con la frase</param>
        /// <param name="choice">choices</param>
        /// <returns>Gramatica</returns>
        private GrammarBuilder CreateGrammarKey(string strFrase, Choices choice)
        {
            SemanticResultKey choiceResultKey = new SemanticResultKey(strFrase, choice);
            GrammarBuilder gb = new GrammarBuilder(choiceResultKey);

            return gb;
        }

        #endregion
    }
}