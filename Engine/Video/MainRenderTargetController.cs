using Engine.Tools;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.Maths;
using Silk.NET.OpenAL;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace Engine.Video
{
    public class MainRenderTargetController(VideoManager videoManager)
    {
        public ViewMode ViewMode { get; private set; } = ViewMode._16_9;

        public event EventHandler OnViewModeChange;

        internal RenderTexture MainRenderTexture;

        internal Vector2 MainRTSize; // Size in screen units (DirectX ones)
        internal Vector2i MainRTPxSize; // Size in pixels

        internal float ResolutionRatio = 1;

        internal Vector2i MainRTOffset;

        private VideoManager videoManager = videoManager;

        private ComPtr<ID3D11Buffer> cB1;
        private ComPtr<ID3D11Buffer> vertexBuffer;

        public void ChangeViewMode(ViewMode viewMode, float resolutionRatio)
        {
            ViewMode = viewMode;
            ResolutionRatio = resolutionRatio;

            Resize();

            OnViewModeChange?.Invoke(this, new EventArgs());
        }


        internal void Resize()
        {
            MainRenderTexture?.Dispose();

            MainRTSize = GetMainRTSize();

            MainRTPxSize = new Vector2i((int)(videoManager.WindowHandler.Size.X * ResolutionRatio * MainRTSize.X), (int)(videoManager.WindowHandler.Size.Y * ResolutionRatio * MainRTSize.Y));

            MainRenderTexture = new RenderTexture();

            MainRenderTexture.Initialize(videoManager.Device, MainRTPxSize);

            // Set CB
            SetCB();

            SetVB();

        }

        internal void SetAsTarget()
        {
            MainRenderTexture.SetAsRenderTarget(videoManager.DeviceContext);
        }

        internal void SetViewPort()
        {
            var viewport = new Viewport(0, 0, MainRTPxSize.X, MainRTPxSize.Y, 0, 1);

            videoManager.DeviceContext.RSSetViewports(1, in viewport);
            
        }


        internal void PrepareDraw()
        {
            Color4 bgColor = new(0, 0, 0, 1);

            // Clear target
            MainRenderTexture.ClearRendertarget(videoManager.DeviceContext, bgColor);

            // Set Viewport
            SetViewPort();

            SetAsTarget();
        }

        internal void Render()
        {
            // Set Shader
            videoManager.BasicEffects.BasicColorTextureShader.ApplyShader();

            // Set texture
            videoManager.DeviceContext.PSSetShaderResources(0, 1, ref MainRenderTexture.GetShaderResourceViewRef());


            // Draw
            videoManager.DeviceContext.Draw(6, 0);
        }

        private unsafe void SetCB()
        {


            Matrix4X4<float> matrix = Matrix4X4<float>.Identity;

            var matrixes = new Matrix4X4<float>[] { matrix };

            cB1.Dispose();

            cB1 = VideoHelper.Buffer.Create(videoManager.Device, ref matrix, 4 * 4 * sizeof(float), BindFlag.ConstantBuffer, 0);

            videoManager.DeviceContext.VSSetConstantBuffers(0, 1, ref cB1);


        }


        private unsafe void SetVB()
        {
            vertexBuffer.Dispose();


            Vector4 color = new(1, 1, 1, 1);

            var leftBot = new VertexPositionColorTexture(new Vector2(-MainRTSize.X, -MainRTSize.Y), color, new Vector2(0, 1));
            var leftTop = new VertexPositionColorTexture(new Vector2(-MainRTSize.X, MainRTSize.Y), color, new Vector2(0, 0));
            var rightTop = new VertexPositionColorTexture(new Vector2(MainRTSize.X, MainRTSize.Y), color, new Vector2(1, 0));
            var rightBot = new VertexPositionColorTexture(new Vector2(MainRTSize.X, -MainRTSize.Y), color, new Vector2(1, 1));

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


            vertexBuffer = VideoHelper.Buffer.Create(videoManager.Device, vertexPositionColorTextures, vertexPositionColorTextures.Length * (int)stride, BindFlag.VertexBuffer, 0);


            videoManager.DeviceContext.IASetVertexBuffers(0, 1, vertexBuffer, in stride, 0);


        }

        private Vector2 GetMainRTSize()
        {
            if (ViewMode == ViewMode.Fullscreen)
            {
                return new Vector2(1, 1);
            }

            float a = 1;

            if (ViewMode == ViewMode._16_9)
                a = 16.0f / 9.0f;
            else if (ViewMode == ViewMode._4_3)
                a = 4.0f / 3.0f;
            else if (ViewMode == ViewMode._16_10)
                a = 16.0f / 10.0f;
            else if (ViewMode == ViewMode._2_1)
                a = 2f;

            Vector2 currentSize = (Vector2)videoManager.WindowHandler.Size;


            float currentRatio = currentSize.X / currentSize.Y;


            if (currentRatio < a)
            {

                return new Vector2(1, currentRatio / a);
            }
            else
            {
                // Window is too Height
                return new Vector2(1 / currentRatio * a, 1);
            }



        }

    }
}
