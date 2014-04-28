using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace StepDX
{
    public partial class Game : Form
    {
        /// <summary>
        /// The DirectX device we will draw on
        /// </summary>
        private Device device = null;
        private Score score;
        private bool end;
        /// <summary>
        /// Height of our playing area (meters)
        /// </summary>
        private float playingH = 4;

        /// <summary>
        /// Width of our playing area (meters)
        /// </summary>
        private float playingW = 32;

        /// <summary>
        /// Vertex buffer for our drawing
        /// </summary>
        private VertexBuffer vertices = null;

        /// <summary>
        /// The background image class
        /// </summary>
        private Background background = null;

        /// <summary>
        /// All of the polygons that make up our world
        /// </summary>
        List<Polygon> world = new List<Polygon>();

        /// <summary>
        /// What the last time reading was
        /// </summary>
        private long lastTime;

        private GameSounds snd;

        /// <summary>
        /// A stopwatch to use to keep track of time
        /// </summary>
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Our player sprite
        /// </summary>
        GameSprite player = new GameSprite();

        /// <summary>
        /// The collision testing subsystem
        /// </summary>
        Collision collision = new Collision();

        //are we at t>0?
        bool beenThanked = false;
        
        /// <summary>
        /// The sky
        /// </summary>
        Sky sky = new Sky();


        public Game()
        {
            InitializeComponent();

            if (!InitializeDirect3D())
                return;
            score = new Score();
            score.start();
            end = false;

            vertices = new VertexBuffer(typeof(CustomVertex.PositionColored), // Type of vertex
                                        4,      // How many
                                        device, // What device
                                        0,      // No special usage
                                        CustomVertex.PositionColored.Format,
                                        Pool.Managed);
            
            background = new Background(device, playingW, playingH);

            // Determine the last time
            stopwatch.Start();
            lastTime = stopwatch.ElapsedMilliseconds;

            Polygon floor = new Polygon();
            floor.AddVertex(new Vector2(0, .1f));
            floor.AddVertex(new Vector2(playingW, .1f));
            floor.AddVertex(new Vector2(playingW, 0));
            floor.AddVertex(new Vector2(0, 0));
            floor.Color = Color.CornflowerBlue;
            world.Add(floor);

            AddObstacle(2, 3, 1.7f, 1.9f, Color.Crimson);
            AddObstacle(4, 4.2f, 1, 2.1f, Color.Coral);
            AddObstacle(5, 6, 2.2f, 2.4f, Color.BurlyWood);
            AddObstacle(5.5f, 6.5f, 3.2f, 3.4f, Color.PeachPuff);
            AddObstacle(6.5f, 7.5f, 2.5f, 2.7f, Color.Chocolate);

            Texture texture = TextureLoader.FromFile(device, "../../stone08.bmp");
            PolygonTextured pt = new PolygonTextured();
            pt.Tex = texture;
            pt.AddVertex(new Vector2(1.2f, 3.5f));
            pt.AddTex(new Vector2(0, 1));
            pt.AddVertex(new Vector2(1.9f, 3.5f));
            pt.AddTex(new Vector2(0, 0));
            pt.AddVertex(new Vector2(1.9f, 3.3f));
            pt.AddTex(new Vector2(1, 0));
            pt.AddVertex(new Vector2(1.2f, 3.3f));
            pt.AddTex(new Vector2(1, 1));
            pt.Color = Color.Transparent;
            world.Add(pt);

            Texture spritetexture = TextureLoader.FromFile(device, "../../sprite_guy.png");
            player.Tex = spritetexture;
            player.AddVertex(new Vector2(-0.2f, 0));
            player.AddTex(new Vector2(0, .8f));
            player.AddVertex(new Vector2(-0.2f, 1));
            player.AddTex(new Vector2(0, 0));
            player.AddVertex(new Vector2(0.2f, 1));
            player.AddTex(new Vector2(0.125f, 0));
            player.AddVertex(new Vector2(0.2f, 0));
            player.AddTex(new Vector2(0.125f, .8f));
            player.Color = Color.Transparent;
            player.Transparent = true;
            player.P = new Vector2(0.5f, .1f);

            Texture skytexture = TextureLoader.FromFile(device, "../../night-sky.png");
            sky.Tex = skytexture;
            sky.AddVertex(new Vector2(0,0));
            sky.AddTex(new Vector2(0,1));
            sky.AddVertex(new Vector2(0,playingH));
            sky.AddTex(new Vector2(0,0));
            sky.AddVertex(new Vector2((float)Width/(float)Height*playingH,playingH));
            sky.AddTex(new Vector2(1,0));
            sky.AddVertex(new Vector2((float)Width/(float)Height*playingH, 0));
            sky.AddTex(new Vector2(1,1));

            AddTriangle(new Vector2(1.5f, 1.1f), 1.5f, .2f, "../../crazy.png");
            AddTriangle(new Vector2(3f, 2.3f), 1, .6f, "../../irontop.png");
            AddTriangle(new Vector2(4f, 2.5f), .8f, .5f, "../../bluething.jpg");
            AddTriangle(new Vector2(5f, 3f), .2f, .4f, "../../stone08.bmp");
            AddTriangle(new Vector2(6f, 2f), 1.2f, .35f, "../../planet.png");
            AddTriangle(new Vector2(7f, 4f), 1.11f, 1, "../../irontop.png");

            
        }

        /// <summary>
        /// Set the sound object for the game
        /// </summary>
        public void setSounds(GameSounds newSnd)
        {
            snd = newSnd;
        }

        /// <summary>
        /// Advance the game in time
        /// </summary>
        public void Advance()
        {

            

            // How much time change has there been?
            long time = stopwatch.ElapsedMilliseconds;
            float delta = (time - lastTime) * 0.001f;       // Delta time in milliseconds
            lastTime = time;

            if (player.P.X < 10)
            {
                snd.SoundSong();
                score.stop();
                end = true;
            }
            else
            {
                if (!beenThanked)
                {
                    beenThanked = true;
                    snd.SoundThanks();
                }
            }
            while (delta > 0)
            {

                float step = delta;
                if (step > 0.05f)
                    step = 0.05f;

                float maxspeed = Math.Max(Math.Abs(player.V.X), Math.Abs(player.V.Y));
                if (maxspeed > 0)
                {
                    step = (float)Math.Min(step, 0.05 / maxspeed);
                }


                
                player.Advance(step);
                if (!(player.P.X - (float)Width / (float)Height * playingH / 2 < 0))
                    sky.Advance(player.P.X - (float)Width/(float)Height * playingH/2);
                else sky.Advance(0);

                foreach (Polygon p in world)
                    p.Advance(step);

                foreach (Polygon p in world)
                {
                    if (collision.Test(player, p))
                    {
                        float depth = collision.P1inP2 ?
                                  collision.Depth : -collision.Depth;
                        player.P = player.P + collision.N * depth;
                        Vector2 v = player.V;
                        if (collision.N.X != 0)
                        {
                            snd.SoundCollision();
                            v.X = 0;
                        }
                        if (collision.N.Y != 0)
                        {
                            v.Y = 0;
                        }
                       if (collision.N.Y < -0.1)
                       {
                           snd.SoundCollision();
                       }
                        player.V = v;
                        player.Advance(0);
                        if (collision.N.Y > 0)
                        {
                            player.STAND = true;
                        }
                    }
                    collision.ClearNormal();
                    if (player.V.Y < -3.0 && player.STAND == false)
                    {
                        snd.SoundDeath();
                    }
                }
                delta -= step;
            }
        }


        public void Render()
        {
            if (device == null)
                return;

            device.Clear(ClearFlags.Target, System.Drawing.Color.Blue, 1.0f, 0);

            int wid = Width;                            // Width of our display window
            int hit = Height;                           // Height of our display window.
            float aspect = (float)wid / (float)hit;     // What is the aspect ratio?

            device.RenderState.ZBufferEnable = false;   // We'll not use this feature
            device.RenderState.Lighting = false;        // Or this one...
            device.RenderState.CullMode = Cull.None;    // Or this one...

            float widP = playingH * aspect;         // Total width of window

            float winCenter = player.P.X;
            if (winCenter - widP / 2 < 0)
                winCenter = widP / 2;
            else if (winCenter + widP / 2 > playingW)
                winCenter = playingW - widP / 2;

            device.Transform.Projection = Matrix.OrthoOffCenterLH(winCenter - widP / 2,
                                                                  winCenter + widP / 2,
                                                                  0, playingH, 0, 1);

            //Begin the scene
            device.BeginScene();
            
            //render the sky
            sky.Render(device);

            // Render the background
            background.Render();

            foreach (Polygon p in world)
            {
                p.Render(device);
            }

            scorebox.Text = score.time().ToString();
            player.Render(device);
            //End the scene
            device.EndScene();
            device.Present();
        }

        /// <summary>
        /// Initialize the Direct3D device for rendering
        /// </summary>
        /// <returns>true if successful</returns>
        private bool InitializeDirect3D()
        {
            try
            {
                // Now let's setup our D3D stuff
                PresentParameters presentParams = new PresentParameters();
                presentParams.Windowed = true;
                presentParams.SwapEffect = SwapEffect.Discard;

                device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);
            }
            catch (DirectXException)
            {
                return false;
            }

            return true;
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close(); // Esc was pressed
            else if (e.KeyCode == Keys.Right)
            {
                Vector2 v = player.V;
                if (player.P.X > 10)
                    v.X = 0f;
                else
                    v.X = 1.5f;
                player.V = v;
            }
            else if (e.KeyCode == Keys.Left)
            {
                Vector2 v = player.V;
                if (player.P.X > 10)
                    v.X = 0f;
                else
                    v.X = -1.5f;
                player.V = v;

            }
            else if (e.KeyCode == Keys.Space && player.STAND == true)
            {
                Vector2 v = player.V;
                if (player.P.X > 10)
                    v.Y = 0f;
                else
                    v.Y = 7;
                player.V = v;
                player.A = new Vector2(0, -9.8f);
                player.STAND = false;
            }

        }

        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
            {
                Vector2 v = player.V;
                v.X = 0;
                player.V = v;
            }
        }

        private void AddObstacle(float left, float right, float bottom, float top, Color color)
        {
            Polygon p = new Polygon();
            p.AddVertex(new Vector2(left, bottom));
            p.AddVertex(new Vector2(left, top));
            p.AddVertex(new Vector2(right, top));
            p.AddVertex(new Vector2(right, bottom));
            p.Color = color;
            world.Add(p);
        }


        private void AddTriangle(Vector2 location, float width, float height, string texture)
        {
            Texture t = TextureLoader.FromFile(device, texture);
            PolygonTextured p = new PolygonTextured();
            p.Tex = t;
            p.AddVertex(new Vector2(location.X - width / 2, location.Y + height / 2));
            p.AddTex(new Vector2(0, 1));

            p.AddVertex(new Vector2(location.X + width / 2, location.Y + height / 2));
            p.AddTex(new Vector2(1, 1));

            p.AddVertex(new Vector2(location.X, location.Y - height / 2));
            p.AddTex(new Vector2(.5f, 0));

            world.Add(p);
        }
    }
}
