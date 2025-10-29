using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Puzzled
{

    ////////////////////////////////////////////////////////////////////////////////////
    // MainMenu
    ////////////////////////////////////////////////////////////////////////////////////
    public partial class MainMenu : UserControl, IScene
    {

        ////////////////////////////////////////////////////////////////////////////////////
        // Constructor & Destructor
        ////////////////////////////////////////////////////////////////////////////////////
        public MainMenu()
        {
            InitializeComponent();
            Loaded += OnLoad;
        }
        ~MainMenu()
        {
            // Note: For future, don't put anything in destructor, since objects are not destroyed at set moment. (GC moment)
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Methods
        ////////////////////////////////////////////////////////////////////////////////////
        private void OnLoad(object sender, RoutedEventArgs args) // Note: We need to do this after layout pass to make sure sizes are calculated
        {
            UserSettings.Load();

            if (s_BootUp)
            {
                Assets.IntroMusic.Play();
                s_BootUp = false;
            }

            m_UIRenderer = new Renderer(UICanvas);
            
            m_DesiredLogoHeight = Game.Instance.Window.Height - c_LogoSize.Y - m_DesiredLogoHeight;
            m_LogoCenterX = (Game.Instance.Window.Width / 2.0f) - (c_LogoSize.X / 2.0f);

            // Logo
            {
                if (!s_AnimationPlayed)
                {
                    // Start at bottom to create the animation
                    m_LogoPosition = new Maths.Vector2(m_LogoCenterX, -c_LogoSize.Y);
                }
                else
                {
                    // Startup animation has already been played so just start at desired height
                    m_LogoPosition = new Maths.Vector2(m_LogoCenterX, m_DesiredLogoHeight);
                }
            }

            Loaded -= OnLoad;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!IsLoaded) return;

            // Title movement
            if (m_LogoPosition.Y < m_DesiredLogoHeight) // Note: The title starts lower (which is higher in this coordinate space)
            {
                m_LogoPosition = new Maths.Vector2(m_LogoPosition.X, m_LogoPosition.Y + (c_UIVelocity * deltaTime));
            }
            else // Note: Only start updating the press start after title reaches height
            {
                if (!Assets.MainMenuMusic.IsPlaying())
                    Assets.MainMenuMusic.Start();

                s_AnimationPlayed = true;

                // Flashing press start
                m_CurrentFlashTimer += deltaTime;
                if (m_CurrentFlashTimer >= c_TimeBetweenFlashes)
                {
                    if (m_PressStartRendered)
                    {
                        PressAnyKey.Foreground = Brushes.Transparent;
                        m_PressStartRendered = false;
                    }
                    else
                    {
                        PressAnyKey.Foreground = Brushes.White;
                        m_PressStartRendered = true;
                    }

                    m_CurrentFlashTimer = 0.0f;
                }
            }
        }

        public void OnRender()
        {
            if (!IsLoaded) return;

            m_UIRenderer.Begin();
            m_UIRenderer.AddQuad(m_LogoPosition, c_LogoSize, Assets.MainMenuLogo);
            m_UIRenderer.End();
        }

        public void OnUIRender()
        {
            if (!IsLoaded) return;
        }

        public void OnEvent(Event e)
        {
            if (!IsLoaded) return;

            void PressCallback()
            {
                if (s_AnimationPlayed)
                {
                    Logger.Info("Pressed start.");
                    Game.Instance.ActiveScene = new SavesMenu(this);
                }
                else
                {
                    // Note: If the startup animation is still playing skip that animation (move title to height)
                    Assets.IntroMusic.CloseAll();
                    m_LogoPosition = new Maths.Vector2(m_LogoPosition.X, m_DesiredLogoHeight);
                }
            }

            // On any event do press()
            if (e is MouseButtonPressedEvent) { PressCallback(); }
            if (e is KeyPressedEvent) { PressCallback(); }

            // Fix Audio lagging when closing
            if (e is WindowCloseEvent)
            {
                Assets.IntroMusic.CloseAll();
                Assets.MainMenuMusic.Stop();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Variables
        ////////////////////////////////////////////////////////////////////////////////////
        private Renderer m_UIRenderer;

        // Animation
        private Maths.Vector2 m_LogoPosition = new Maths.Vector2(0.0f, 0.0f);
        
        private float m_DesiredLogoHeight = 100.0f; // Note: This is the size from the top of the window, it get properly calculated in the constructor.
        private float m_LogoCenterX;

        private float m_CurrentFlashTimer = 0.0f;
        private bool m_PressStartRendered = false;

        ////////////////////////////////////////////////////////////////////////////////////
        // Static variables
        ////////////////////////////////////////////////////////////////////////////////////
        private static bool s_BootUp = true;
        private static bool s_AnimationPlayed = false;

        private const float c_UIVelocity = 67.45f; // Matches Intro.wav
        private static readonly Maths.Vector2 c_LogoSize = new Maths.Vector2(156.0f * 3.0f, 79.0f * 3.0f); // Note: 156x79 are the dimensions of the logo.
        private const float c_TimeBetweenFlashes = 0.4f;

    }

}
