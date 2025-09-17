﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Puzzled
{

    //////////////////////////////////////////////////////////////////////////////////
    // Renderer
    //////////////////////////////////////////////////////////////////////////////////
    public class Renderer
    {
    
        ////////////////////////////////////////////////////////////////////////////////////
        // Internal structs
        ////////////////////////////////////////////////////////////////////////////////////
        internal struct Quad
        {
            public Maths.Vector2 Size;
            public Maths.Vector2 Position;
    
            public Texture TextureReference;
            public UV TextureCoords;
        }
    
        internal class VisualHost : FrameworkElement
        {
            private readonly Visual m_Visual;
            public VisualHost(Visual visual) => m_Visual = visual;
    
            protected override int VisualChildrenCount => 1;
            protected override Visual GetVisualChild(int index) => m_Visual;
        }
    
        ////////////////////////////////////////////////////////////////////////////////////
        // Constructor & Destructor
        ////////////////////////////////////////////////////////////////////////////////////
        public Renderer(Canvas canvas)
        {
            m_Quads = new List<Quad>();
            m_Canvas = canvas;

            m_Visual = new DrawingVisual();
            m_VisualHost = new VisualHost(m_Visual);
            
            m_Canvas.Children.Add(m_VisualHost);
        }
        ~Renderer()
        {
        }
    
        ////////////////////////////////////////////////////////////////////////////////////
        // Methods
        ////////////////////////////////////////////////////////////////////////////////////
        public void Begin()
        {
            m_Quads.Clear();
        }
    
        public void End()
        {
            using (DrawingContext dc = m_Visual.RenderOpen())
            {
                foreach (Quad quad in m_Quads)
                {
                    CroppedBitmap cropped = new CroppedBitmap(
                        quad.TextureReference.GetInternalImage(),
                        new Int32Rect((int)quad.TextureCoords.X, (int)quad.TextureCoords.Y, (int)quad.TextureCoords.Width, (int)quad.TextureCoords.Height) // UV rectangle in pixels
                    );
                    cropped.Freeze(); // TODO: Optimize cropping

                    dc.DrawImage(cropped, new Rect(quad.Position.X, quad.Position.Y, quad.Size.X, quad.Size.Y));
                }
            }
        }
    
        public void AddQuad(Maths.Vector2 position, Maths.Vector2 size, Texture texture) { AddQuad(position, size, texture, new UV(texture)); }
        public void AddQuad(Maths.Vector2 position, Maths.Vector2 size, Texture texture, UV textureCoords) 
        { 
            m_Quads.Add(new Quad
            { 
                Position = new Maths.Vector2(position.X, Game.Instance.Window.Height - size.Y - position.Y), 
                Size = size, 
                TextureReference = texture, 
                TextureCoords = textureCoords 
            }); 
        }
        ////////////////////////////////////////////////////////////////////////////////////
        // Variables
        ////////////////////////////////////////////////////////////////////////////////////
        private List<Quad> m_Quads;
        private Canvas m_Canvas;

        private DrawingVisual m_Visual;
        private VisualHost m_VisualHost;
    
    }

}
