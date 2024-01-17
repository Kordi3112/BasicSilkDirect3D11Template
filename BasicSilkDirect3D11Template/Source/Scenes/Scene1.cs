using Engine.Core;
using Engine.Video;
using SharpDX.Direct2D1;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.Maths;
using System;


namespace BasicSilkDirect3D11Template.Source.Scenes
{
    internal class Scene1 : Scene
    {


        public Scene1() : base(new SceneInfo("Scene1", 1))
        {

        }


        public override void Init()
        {
            Console.WriteLine("Init");
        }

        public override void Load()
        {
            Console.WriteLine("Load");
        }

        public override void PreUpdate(GTime deltaTime)
        {

        }

        public override void FixedUpdate(GTime deltaTime)
        {

        }

        public override void Update(GTime deltaTime)
        {

        }

        public override void LateUpdate(GTime deltaTime)
        {

        }

        public override void Render(GTime deltaTime)
        {
            //DrawTest();

        }



        public override void OnResized()
        {

        }

        public override void Close()
        {

        }


        private unsafe void DrawTest()
        {
            var videoManager = GameManager.VideoManager;

            Matrix4X4<float> matrix = Matrix4X4<float>.Identity;

            var cB1 = VideoHelper.Buffer.Create(GameManager.VideoManager.Device, ref matrix, 4 * 4 * sizeof(float), BindFlag.ConstantBuffer, 0);

            GameManager.VideoManager.DeviceContext.VSSetConstantBuffers(0, 1, ref cB1);

            videoManager.DeviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3D11PrimitiveTopologyTrianglelist);

            var color = new Vector4D<float>(1, 0, 0, 1);

            var size = new Vector2D<float>(0.5f);

            var leftBot = new VertexPositionColorTexture(new Vector2D<float>(-size.X, -size.Y), color, new Vector2D<float>());
            var leftTop = new VertexPositionColorTexture(new Vector2D<float>(-size.X, size.Y), color, new Vector2D<float>());
            var rightTop = new VertexPositionColorTexture(new Vector2D<float>(size.X, size.Y), color, new Vector2D<float>());
            var rightBot = new VertexPositionColorTexture(new Vector2D<float>(size.X, -size.Y), color, new Vector2D<float>());

            VertexPositionColorTexture[] vertexPositionColorTextures =
                [
                    leftBot,
                    leftTop,
                    rightTop,

                    leftBot,
                    rightTop,
                    rightBot,
                ];

            uint stride = (uint)VertexPositionColorTexture.SizeInBytes();

            var vertexBuffer = VideoHelper.Buffer.Create(videoManager.Device, vertexPositionColorTextures, vertexPositionColorTextures.Length * (int)stride, BindFlag.VertexBuffer, 0);


            videoManager.DeviceContext.IASetVertexBuffers(0, 1, vertexBuffer, in stride, 0);

            // Set Shader
            videoManager.BasicEffects.BasicColorTextureShader.ApplyShader();

            // Draw
            videoManager.DeviceContext.Draw(6, 0);

            ///
            cB1.Dispose();
            vertexBuffer.Dispose();

        }
    }
}
