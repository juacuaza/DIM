using System;
using System.Drawing;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;
using global::AppSpeech.Properties;

namespace AppSpeech
{
    public partial class App : Form
    {
        #region Atributos

        private System.Speech.Recognition.SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        private SpeechSynthesizer synth = new SpeechSynthesizer();
        private Timer timer;
        private int iSegundos;
        private bool Bloqueo;

        #endregion

        #region Constructor

        public App()
        {
            InitializeComponent();
            this.WMP.Visible = false;
            this.LBL_Hora.Visible = false;
            this.PB_Alarma.Visible = false;
            Browser.Visible = false;            
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
            this.BackgroundImage = Resources.bloqueo; 
        }



        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            RecognizedWordUnit strOpcion;

            //obtenemos un diccionario con los elementos semánticos
            SemanticValue semantics = e.Result.Semantics;

            strOpcion = e.Result.Words[e.Result.Words.Count - 1];
            string rawText = e.Result.Text;
            RecognitionResult result = e.Result;

            if (strOpcion.Text.ToLower().Contains("desbloquear") || (strOpcion.Text.ToLower().Contains("bloquear")))
            {
                Bloqueo = (bool)semantics["bloq"].Value;
            }

            if (Bloqueo)
            {                
                switch (strOpcion.Text.ToLower())
                {
                    case "desbloquear":
                        synth.Speak("Telefono desbloqueado");
                        this.LBL_Texto.Text = "Telefono desbloqueado";
                        this.BackgroundImage = Resources.principal;
                        Update();
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
                            //this.LBL_Texto.Text = "La alarma sonará en " + semantics["alarma"].Value.ToString() + " segundos";
                            this.BackgroundImage = Resources.reloj;
                            this.LBL_Texto.Text = rawText;
                            Update();
                            synth.Speak("La alarma sonará en " + semantics["alarma"].Value.ToString() + " segundos");
                            PonerAlarma((int)semantics["alarma"].Value);
                        }
                        else if (rawText.ToLower().Contains("quitar"))
                        {
                            if (this.timer != null)
                            {
                                this.timer.Stop();
                                this.BackgroundImage = Resources.principal;                                
                                this.LBL_Texto.Text = rawText;
                                Update();
                                synth.Speak("Alarma quitada");
                                WMP.Ctlcontrols.stop();
                                PB_Alarma.Visible = false;                                
                            }

                        }
                        break;

                    case "video":
                    case "musica":
                    /*case "navegador":*/
                        if (rawText.ToLower().Contains("abrir") || rawText.ToLower().Contains("poner"))
                        {
                            abrirAplicacion(strOpcion.Text.ToLower());
                            this.LBL_Texto.Text = rawText;
                            synth.Speak(rawText);
                        }
                        else if (rawText.ToLower().Contains("cerrar") || rawText.ToLower().Contains("quitar"))
                        {
                            cerrarAplicacion(strOpcion.Text.ToLower());
                            this.LBL_Texto.Text = rawText;
                            synth.Speak(rawText);
                        }
                        break;

                    case "tiempo":
                        if (rawText.ToLower().Contains("abrir"))
                        {
                            this.BackgroundImage = Resources.tiempo;                            
                            synth.Speak("El tiempo estara nublado, abrigate");
                            this.LBL_Texto.Text = rawText;
                            Update();
                        }
                        else if(rawText.ToLower().Contains("cerrar"))
                        {
                            this.BackgroundImage = Resources.principal;                            
                            synth.Speak("Cerrando tiempo");
                            this.LBL_Texto.Text = rawText;
                            Update();
                        }
                        break;


                    case "reloj":
                        if (rawText.ToLower().Contains("abrir"))
                        {
                            this.BackgroundImage = Resources.reloj;
                            this.LBL_Hora.Visible = true;          
                            this.LBL_Hora.Text = DateTime.Now.ToString("HH:MM");
                            this.LBL_Texto.Text = rawText;
                            Update();
                            synth.Speak("son las " + this.LBL_Hora.Text);                            
                        }
                        else if (rawText.ToLower().Contains("cerrar"))
                        {
                            this.BackgroundImage = Resources.principal;
                            this.LBL_Hora.Visible = false;                            
                            this.LBL_Texto.Text = rawText;
                            Update();
                            synth.Speak("Cerrando reloj");                            
                        }
                        break;

                    case "viajes":
                    case "coches":
                    case "noticias":
                        if (rawText.ToLower().Contains("buscar"))
                        {
                            this.LBL_Texto.Text = rawText;
                            Browser.Visible = true;
                            synth.Speak("Buscando "+ strOpcion.Text);                            
                            AbrirBusqueda(semantics["busqueda"].Value.ToString());
                            Update();
                        }
                        else if (rawText.ToLower().Contains("cerrar"))
                        {
                            this.LBL_Texto.Text = rawText;
                            synth.Speak(rawText);
                            Browser.Visible = false;
                            Update();                            
                        }

                        break;

                    default:
                        this.LBL_Texto.Text = "No info provided.";
                        break;
                }
            }
            else
            {
                if (strOpcion.Text.ToLower().Contains("bloquear"))
                {
                    synth.Speak("Telefono bloqueado");
                    this.LBL_Texto.Text = "Telefono bloqueado";
                    this.BackgroundImage = Resources.bloqueo;
                    Update();
                }
                else if (strOpcion.Text.ToLower().Contains("salir"))
                {
                    this.LBL_Texto.Text = semantics["salir"].Value.ToString();
                    Update();
                    synth.Speak("Hasta la próxima");
                    this.Close();
                }
                else
                {
                    synth.Speak("Tienes que desbloquear el telefono");
                    this.LBL_Texto.Text = "Tienes que desbloquear el telefono";
                }
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
            else if(iSegundos == -1)
            {
                WMP.URL = @"Resource\Despertador.mp3";
                WMP.Ctlcontrols.play();
                //PB_Alarma.Image = Resources.Alarma;
                PB_Alarma.Visible = true;
            }
            
        }

        private void abrirAplicacion(string strOpcion)
        {
            try
            {
                switch (strOpcion)
                {
                    case "video":
                        WMP.Visible = true;
                        WMP.Location = new Point(12, 20);
                        WMP.URL = @"Resource\Documental.mp4";
                        WMP.Ctlcontrols.play();
                        break;
                    case "musica":
                        WMP.Visible = true;
                        WMP.Location = new Point(12, 20);
                        WMP.URL = @"Resource\Cancion.mp3";
                        WMP.Ctlcontrols.play();
                        break;
                    case "navegador":
                        //Process myProcess = new Process();
                        //try
                        //{
                        //    // true is the default, but it is important not to set it to false
                        //    myProcess.StartInfo.UseShellExecute = true;
                        //    myProcess.StartInfo.FileName = "http://www.google.es";
                        //    myProcess.Start();
                        //}
                        //catch (Exception e)
                        //{
                        //    Console.WriteLine(e.Message);
                        //}
                        break;
                }
            }
            catch (Exception e)
            {
                this.LBL_Texto.Text = e.Message;
            }
        }

        private void cerrarAplicacion(string strOpcion)
        {
            try
            {
                switch (strOpcion)
                {
                    case "video":
                        if (WMP != null)
                        {
                            WMP.Ctlcontrols.stop();
                            WMP.Visible = false;
                        }
                        break;
                    case "musica":
                        if (WMP != null)
                        {
                            WMP.Ctlcontrols.stop();
                            WMP.Visible = false;
                        }
                        break;
                    case "navegador":
                        //try
                        //{
                        //    foreach (Process proc in Process.GetProcesses())
                        //    {
                        //        if (proc.ProcessName == "MicrosoftEdge" || proc.ProcessName == "iexplore")
                        //            proc.Kill();
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    this.LBL_Texto.Text = ex.Message;
                        //}
                        break;
                }
            }
            catch(Exception e)
            {
                this.LBL_Texto.Text = e.Message;
            }
        }

        
        private void AbrirBusqueda(string strBusqueda)
        {
            Browser.ScriptErrorsSuppressed = true;
            Browser.Navigate(strBusqueda,
                          null,
                          null,
                          "User-Agent:Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Grammar CreateGrammarBuilderSemantics(params int[] info)
        {
            synth.Speak("Creando ahora la gramática");

            ///Palabra inicial
            GrammarBuilder oyedima = "Oye DIMA";
            GrammarBuilder comoTe= "Como te";
            GrammarBuilder poner = "Poner";
            GrammarBuilder quitar = "Quitar";
            GrammarBuilder abrir = "Abrir";
            GrammarBuilder cerrar = "Cerrar";
            GrammarBuilder buscar = "Buscar";                 
            
            //Choices con la palabra final
            Choices cerrarChoices = CreateChoiceSalir();
            Choices nombreChoice = CreateChoicesNombre();
            Choices alarmaChoice = CreateChoicesAlarma();
            Choices appChoice = CreateChoicesAplicacion();
            Choices bloqueoChoice = CreateChoicesBloque();
            Choices tiempoChoice = CreateChoiceTiempo();
            Choices relojChoice = CreateChoicesReloj();
            Choices buscarChoice = CreateChoiceBuscar();

            //Grammar con las key
            GrammarBuilder gSalir = CreateGrammarKey("salir", cerrarChoices);
            GrammarBuilder gNombre = CreateGrammarKey("llamas", nombreChoice);
            GrammarBuilder galarma = CreateGrammarKey("alarma", alarmaChoice);
            GrammarBuilder gApp = CreateGrammarKey("app", appChoice);
            GrammarBuilder gBloqueo = CreateGrammarKey("bloq", bloqueoChoice);
            GrammarBuilder gTiempo = CreateGrammarKey("temp", tiempoChoice);
            GrammarBuilder gReloj = CreateGrammarKey("time", relojChoice);
            GrammarBuilder gBuscar = CreateGrammarKey("busqueda", buscarChoice);

            //Prefijo de la frase
            Choices cOyedima = new Choices(oyedima);

            //Frase nombre
            Choices cnombre = new Choices(comoTe);
            GrammarBuilder fNombre = new GrammarBuilder(cnombre);
            fNombre.Append(gNombre);

            //frase alarma
            Choices calarma = new Choices(poner, quitar);
            GrammarBuilder fPalarma = new GrammarBuilder(calarma);
            fPalarma.Append(galarma);

            //frase aplicacion 
            Choices capp = new Choices(abrir, poner, cerrar, quitar);
            GrammarBuilder fapp = new GrammarBuilder(capp);
            fapp.Append(gApp);

            //frase bloqueo
            GrammarBuilder fbloqueo = new GrammarBuilder(cOyedima);
            fbloqueo.Append(gBloqueo);

            //frase tiempo
            Choices ctiempoAbrir = new Choices(abrir, cerrar);
            GrammarBuilder ftiempo = new GrammarBuilder(ctiempoAbrir);            
            ftiempo.Append(gTiempo);

            //frase hora            
            Choices creloj = new Choices(abrir, cerrar);
            GrammarBuilder freloj = new GrammarBuilder(creloj);            
            freloj.Append(gReloj);

            Choices cbuscar = new Choices(buscar, cerrar);
            GrammarBuilder fbuscar = new GrammarBuilder(cbuscar);
            fbuscar.Append(gBuscar);

            //Frase final
            Choices cFinal = new Choices(fNombre, gSalir, fPalarma, fapp, fbloqueo, ftiempo, freloj, fbuscar);
            GrammarBuilder fFinal = new GrammarBuilder(cFinal);

            try
            {
                //Grammar grammar = new Grammar(frase1);
                Grammar grammar = new Grammar(fFinal);
                grammar.Name = "Oye DIMA/Poner";
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
        private Choices CreateChoiceBuscar()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("Viajes", "https://www.kayak.es/flights");
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Coches", "http://www.motorpasion.com");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Noticias", "http://www.20minutos.es");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
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

            SemanticResultValue choiceResultValue = new SemanticResultValue("Alarma", 10);
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
        }

        /// <summary>
        /// Crea el Choice del tiempo
        /// </summary>
        /// <returns>Choice del tiempo</returns>
        private Choices CreateChoiceTiempo()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("Tiempo", "Tiempo");
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Choices CreateChoicesBloque()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("bloquear", false);
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);
            choiceResultValue = new SemanticResultValue("desbloquear", true);
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            return Choice;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Choices CreateChoicesReloj()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("reloj", "hora");
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
        /// Crea el Choice de las aplicaciones
        /// </summary>
        /// <returns></returns>
        private Choices CreateChoicesAplicacion()
        {
            Choices Choice = new Choices();

            SemanticResultValue choiceResultValue = new SemanticResultValue("Musica", "Cancion");
            GrammarBuilder resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Video", "Audio");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            Choice.Add(resultValueBuilder);

            //choiceResultValue = new SemanticResultValue("Navegador", "Internet");
            //resultValueBuilder = new GrammarBuilder(choiceResultValue);
            //Choice.Add(resultValueBuilder);

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