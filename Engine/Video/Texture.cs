using Engine.Core;
using SharpDX.WIC;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using System;

using System.Runtime.CompilerServices;


namespace Engine.Video
{
    internal class Texture : IDisposable
    {
        internal ComPtr<ID3D11Texture2D> texture;

        private ComPtr<ID3D11ShaderResourceView> shaderResourceView;

        public Vector2i Size { get; private set; }

        public ref ComPtr<ID3D11ShaderResourceView> GetShaderResourceViewRef() => ref shaderResourceView;


        public unsafe bool Create(ComPtr<ID3D11Device> device, Stream stream, Vector2i size)
        {
            Size = size;

            using (ImagingFactory2 imagingFactory = new ImagingFactory2())
            {
                var bitmapSource = TextureLoader.LoadBitmap(imagingFactory, stream);

                //Texture2D = TextureLoader.CreateTexture2DFromBitmap(device, bitmapSource);

                texture = TextureLoader.CreateTexture(device,size, TextureLoader.GetByteArrayFromBitmapSource(bitmapSource));
            };

            CreateShaderResourceView(device);

            return true;
        }

        public unsafe bool Create(ComPtr<ID3D11Device> device, Vector2i size, byte[] data)
        {
            Size = size;

            texture = TextureLoader.CreateTexture(device, size, data);

            CreateShaderResourceView(device);

            return true;
        }

        public unsafe bool Create(ComPtr<ID3D11Device> device, Vector2i size, Color[] colors) => Create(device, size, TextureLoader.GetByteArrayFromColors(colors));

        public unsafe bool Create(ComPtr<ID3D11Device> device, Vector2i size, Func<int, Color> func)
        {
            var colors = new Color[size.X * size.Y];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = func(i);
            }

            return Create(device, size, colors);

        }

        public unsafe bool Create(ComPtr<ID3D11Device> device, Vector2i size, Func<int, int, Color> func)
        {
            var colors = new Color[size.X * size.Y];

            for (int i = 0; i < colors.Length; i++)
            {
                int y = i / size.X;
                int x = i % size.X;

                colors[i] = func(x, y);
            }

            return Create(device, size, colors);
        }

        public unsafe static ComPtr<ID3D11Texture2D> CreateStagged(ComPtr<ID3D11Device> device, Vector2i size)
        {
            ComPtr<ID3D11Texture2D> texture = default;

            var texture2DDesc = new Texture2DDesc()
            {
                Width = (uint)size.X,
                Height = (uint)size.Y,
                ArraySize = 1,
                BindFlags = (uint)BindFlag.None,
                Usage = Usage.Staging,
                CPUAccessFlags = (uint)CpuAccessFlag.Read,
                Format = Format.FormatR8G8B8A8Unorm,
                MipLevels = 1,
                MiscFlags = (uint)ResourceMiscFlag.None,
                SampleDesc = new SampleDesc(1, 0),
            };

            CrashHandler.CheckForError(device.CreateTexture2D(texture2DDesc, Unsafe.NullRef<SubresourceData>(), ref texture), "Stagged Texture creation error");

            return texture;
        }


        private unsafe void CreateShaderResourceView(ComPtr<ID3D11Device> device)
        {
            var shaderResourceViewDesc = new ShaderResourceViewDesc()
            {
                Format = Format.FormatR8G8B8A8Unorm,
                ViewDimension = D3DSrvDimension.D3D11SrvDimensionTexture2D,
            };

            shaderResourceViewDesc.Texture2D.MipLevels = 1;
            shaderResourceViewDesc.Texture2D.MostDetailedMip = 0;

            SilkMarshal.ThrowHResult(device.CreateShaderResourceView(texture, &shaderResourceViewDesc, ref shaderResourceView));
        }

        public void Dispose()
        {
            shaderResourceView.Dispose();

            texture.Dispose();
        }
    }


}
