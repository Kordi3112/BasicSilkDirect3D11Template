using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.Maths;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Video
{
    public class RenderTexture : IDisposable
    {
        private ComPtr<ID3D11Texture2D> texture;
        private ComPtr<ID3D11RenderTargetView> renderTargetView;
        private ComPtr<ID3D11ShaderResourceView> shaderResourceView;

        public ref ComPtr<ID3D11ShaderResourceView> GetShaderResourceViewRef() => ref shaderResourceView;
        public ComPtr<ID3D11ShaderResourceView> GetShaderResourceView() => shaderResourceView;

        public unsafe bool Initialize(ComPtr<ID3D11Device> device, Vector2i size)
        {
            try
            {

                var textureDesc = new Texture2DDesc()
                {
                    Width = (uint)size.X,
                    Height = (uint)size.Y,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.FormatR8G8B8A8Unorm, // Format.R8G8B8A8_UNorm
                    SampleDesc = new SampleDesc(1,0),
                    Usage = Usage.Default,
                    //BindFlags = (uint)(BindFlag.RenderTarget | BindFlag.ShaderResource),
                    BindFlags = (uint)BindFlag.RenderTarget | (uint)BindFlag.ShaderResource,
                    //BindFlags = 32 | 8, //BindFlags.RenderTarget | BindFlags.ShaderResource
                    CPUAccessFlags = 0, //CpuAccessFlags.None
                    MiscFlags = 0, //ResourceOptionFlags.None

                };

               

                SilkMarshal.ThrowHResult(device.CreateTexture2D(textureDesc, null, ref texture));
                


                var renderTargetDesc = new RenderTargetViewDesc()
                {
                    Format = textureDesc.Format,
                    ViewDimension = RtvDimension.Texture2D,
                    
                };

                SilkMarshal.ThrowHResult(device.CreateRenderTargetView(texture, renderTargetDesc, ref renderTargetView));

                var shaderResourceViewDesc = new ShaderResourceViewDesc()
                {
                    Format = textureDesc.Format,
                    ViewDimension = D3DSrvDimension.D3D11SrvDimensionTexture2D,
                };

                shaderResourceViewDesc.Texture2D.MipLevels = 1;
                shaderResourceViewDesc.Texture2D.MostDetailedMip = 0;

                SilkMarshal.ThrowHResult(device.CreateShaderResourceView(texture, shaderResourceViewDesc, ref shaderResourceView));

                

                
                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Error");
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        public void SetAsRenderTarget(ComPtr<ID3D11DeviceContext> context)
        {
            unsafe
            {

                context.OMSetRenderTargets(1, renderTargetView, ref Unsafe.NullRef<ID3D11DepthStencilView>());
            }           
        }

        public void ClearRendertarget(ComPtr<ID3D11DeviceContext> context, Color4 color)
        {
            unsafe
            {
                Span<float> span = new Span<float>([1, 0, 0, 1]);

                context.ClearRenderTargetView(renderTargetView, span);
            }
        }

        public void SetShaderResource(ComPtr<ID3D11DeviceContext> context, int slot)
        {

            
        }



        public void Dispose()
        {
            shaderResourceView.Dispose();
            renderTargetView.Dispose();
            texture.Dispose();
        }
    }
}
