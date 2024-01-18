using Engine.Core;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D.Compilers;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Windowing;
using System.Runtime.CompilerServices;


namespace Engine.Video
{
    /// <summary>
    /// Handling drawing processes
    /// </summary>
    public class VideoManager : IDisposable
    {

        public BasicShaders BasicEffects { get; private set; }

        public ComPtr<ID3D11Device> Device => device;
        public ComPtr<ID3D11DeviceContext> DeviceContext => deviceContext;

        private ComPtr<IDXGISwapChain1> swapchain = default;
        private ComPtr<ID3D11Device> device = default;
        private ComPtr<ID3D11DeviceContext> deviceContext = default;

        ComPtr<ID3D11SamplerState> sampler = default;

        // Load the DXGI and Direct3D11 libraries for later use.
        // Given this is not tied to the window, this doesn't need to be done in the OnLoad event.
        private DXGI dxgi = null!;
        private D3D11 d3d11 = null!;
        private D3DCompiler compiler = null!;

        private ComPtr<ID3D11Texture2D> backBuffer;
        private ComPtr<ID3D11RenderTargetView> swapChainRTV;

        public MainRenderTargetController MainRenderTargetController { get; private set; }

        private GameManager gameManager;

        public IWindow WindowHandler => gameManager.WindowHandler;

        internal VideoManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            BasicEffects = new BasicShaders();

            MainRenderTargetController = new MainRenderTargetController(this);
        }


        internal unsafe void Initialize(IWindow window)
        {

            //Whether or not to force use of DXVK on platforms where native DirectX implementations are available
            const bool forceDxvk = false;

            dxgi = DXGI.GetApi(window, forceDxvk);
            d3d11 = D3D11.GetApi(window, forceDxvk);
            compiler = D3DCompiler.GetApi();

            // Create our D3D11 logical device.
            CrashHandler.CheckForError
            (
                d3d11.CreateDevice
                (
                    default(ComPtr<IDXGIAdapter>),
                    D3DDriverType.Hardware,
                    Software: default,
                    (uint)CreateDeviceFlag.Debug,
                    null,
                    0,
                    D3D11.SdkVersion,
                    ref device,
                    null,
                    ref deviceContext
                )
            , "Device creation failure");


            // Create SwapChain
            var swapChainDesc = new SwapChainDesc1
            {
                BufferCount = 2, // double buffered
                Format = Format.FormatR8G8B8A8Unorm,
                BufferUsage = DXGI.UsageRenderTargetOutput,
                SwapEffect = SwapEffect.FlipDiscard,
                SampleDesc = new SampleDesc(1, 0)
                
            };


            // Create our DXGI factory to allow us to create a swapchain. 
            var factory = dxgi.CreateDXGIFactory<IDXGIFactory2>();

            // Create the swapchain.
            CrashHandler.CheckForError
            (
                factory.CreateSwapChainForHwnd
                (
                    device,
                    window.Native!.DXHandle!.Value,
                    in swapChainDesc,
                    null,
                    ref Unsafe.NullRef<IDXGIOutput>(),
                    ref swapchain
                )
            , "Swap chain creation failure");

            // Load
            BasicEffects.Load(device);

            Console.WriteLine("Devices Inited");
        }


        private void PrepareFinalRender()
        {
            int currentFilter = 129;

            Color4 borderColor = new(0, 0, 0, 0);

            var samplerDesc = new SamplerDesc()
            {

                //Filter = (Filter)currentFilter,
                Filter = Filter.MaximumMinLinearMagMipPoint,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                MipLODBias = 0,
                MaxAnisotropy = 1,
                ComparisonFunc = ComparisonFunc.Always,
                MinLOD = 0,
                MaxLOD = float.MaxValue,
            };

            sampler.Dispose();

            SilkMarshal.ThrowHResult(device.CreateSamplerState(samplerDesc, ref sampler));

            deviceContext.PSSetSamplers(0, 1, ref sampler);



        }

        internal unsafe void FinalRender()
        {
            PrepareFinalRender();
            // Set target

            Color4 bgColor = new(0f, 0f, 0f, 0.0f);
            deviceContext.ClearRenderTargetView(swapChainRTV, bgColor.GetPtr());

            // Tell the output merger about our render target view.
            SetSwapChainBufferAsRenderTarget();

            deviceContext.IASetPrimitiveTopology(D3DPrimitiveTopology.D3D11PrimitiveTopologyTrianglelist);

            // Set viewPort
            var viewport = new Viewport(0, 0, gameManager.WindowHandler.FramebufferSize.X, gameManager.WindowHandler.FramebufferSize.Y);
            deviceContext.RSSetViewports(1, in viewport);


            // Render Main Target
            MainRenderTargetController.Render();


            // Present the drawn image.
            swapchain.Present(1, 0);
        }

        internal unsafe void OnFramebufferResize(Vector2i newSize)
        {
            Console.WriteLine(newSize);

            backBuffer.Dispose();
            swapChainRTV.Dispose();

            SilkMarshal.ThrowHResult
            (
                swapchain.ResizeBuffers(0, (uint)newSize.X, (uint)newSize.Y, Format.FormatB8G8R8A8Unorm, 0)
            );

            backBuffer = swapchain.GetBuffer<ID3D11Texture2D>(0);

            CrashHandler.CheckForError(device.CreateRenderTargetView(backBuffer, null, ref swapChainRTV), "Swapchain RenderView creation failure");

            // Resize main target
            MainRenderTargetController.Resize();

            // Define rasterizer
            var rsDesc = new RasterizerDesc()
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                FrontCounterClockwise = false,
                DepthBias = 0,
                DepthBiasClamp = 0,
                SlopeScaledDepthBias = 0,
                DepthClipEnable = false,
                // ScissorEnable = true require defining of scissors
                ScissorEnable = false,
                MultisampleEnable = true,
                AntialiasedLineEnable = true,
            };

            // Create Rasterizer
            ComPtr<ID3D11RasterizerState> rs = default;

            CrashHandler.CheckForError(device.CreateRasterizerState(rsDesc, ref rs), "RasterizerState creation failure");


            // Set Rasterizer State
            deviceContext.RSSetState(rs);



        }


        internal void SetSwapChainBufferAsRenderTarget() => deviceContext.OMSetRenderTargets(1, ref swapChainRTV, ref Unsafe.NullRef<ID3D11DepthStencilView>());

        public void Dispose()
        {
            backBuffer.Dispose();
            swapChainRTV.Dispose();

            swapchain.Dispose();

            device.Dispose();
        }


    }
}
