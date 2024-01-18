using Engine.Core;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Tools
{
    public class VideoHelper
    {
        public class Buffer
        {

            public unsafe static void UpdateDynamicResource<T>(ComPtr<ID3D11DeviceContext> context, ComPtr<ID3D11Buffer> resource, ref T data, int sizeInBytes) where T : unmanaged
            {
                var mappedSubresource = new MappedSubresource();

                context.Map(resource, 0, Map.WriteDiscard, 0, ref mappedSubresource);

                fixed (T* pData = &data)
                {
                    System.Buffer.MemoryCopy(pData, mappedSubresource.PData, sizeInBytes, sizeInBytes);
                }


            }

            public unsafe static void UpdateDynamicResource<T>(ComPtr<ID3D11DeviceContext> context, ComPtr<ID3D11Buffer> resource, T[] data, int sizeInBytes) where T : unmanaged
            {
                var mappedSubresource = new MappedSubresource();

                context.Map(resource, 0, Map.WriteDiscard, 0, ref mappedSubresource);

                fixed (T* pData = data)
                {
                    System.Buffer.MemoryCopy(pData, mappedSubresource.PData, sizeInBytes, sizeInBytes);
                }


            }

            public static void UpdateSubresource<T>(ComPtr<ID3D11DeviceContext> context, ComPtr<ID3D11Buffer> resource, ref T data) where T : unmanaged
            {
                context.UpdateSubresource(resource, 0, Unsafe.NullRef<Box>(), in data, 0, 0);
            }

            public unsafe static void UpdateSubresource<T>(ComPtr<ID3D11DeviceContext> context, ComPtr<ID3D11Buffer> resource, T[] data) where T : unmanaged
            {
                fixed (T* pData = data)
                {
                    context.UpdateSubresource(resource, 0, Unsafe.NullRef<Box>(), pData, 0, 0);
                }
            }

            public static ComPtr<ID3D11Buffer> CreateDynamic(ComPtr<ID3D11Device> device, int byteWidth, BindFlag bindFlag, int structureByteStride)
            {
                ComPtr<ID3D11Buffer> buffer = default;

                var bufferDesc = new BufferDesc
                {
                    ByteWidth = (uint)byteWidth,
                    Usage = Usage.Dynamic,
                    BindFlags = (uint)bindFlag,
                    CPUAccessFlags = (uint)CpuAccessFlag.Write,
                    MiscFlags = (uint)ResourceMiscFlag.None,
                    StructureByteStride = (uint)structureByteStride,
                };




                unsafe
                {
                    CrashHandler.CheckForError(device.CreateBuffer(in bufferDesc, null, ref buffer), "Failed buffer creation");
                }


                return buffer;

            }

            public static ComPtr<ID3D11Buffer> CreateEmpty(ComPtr<ID3D11Device> device, int byteWidth, BindFlag bindFlag, int structureByteStride)
            {
                ComPtr<ID3D11Buffer> buffer = default;

                var bufferDesc = new BufferDesc
                {
                    ByteWidth = (uint)byteWidth,
                    Usage = Usage.Default,
                    BindFlags = (uint)bindFlag,
                    CPUAccessFlags = (uint)CpuAccessFlag.None,
                    MiscFlags = (uint)ResourceMiscFlag.None,
                    StructureByteStride = (uint)structureByteStride,
                };




                unsafe
                {
                    CrashHandler.CheckForError(device.CreateBuffer(in bufferDesc, null, ref buffer), "Failed buffer creation");
                }


                return buffer;

            }

            public static ComPtr<ID3D11Buffer> Create<T>(ComPtr<ID3D11Device> device, T[] data, int byteWidth, BindFlag bindFlag, int structureByteStride) where T : unmanaged
            {
                ComPtr<ID3D11Buffer> buffer = default;

                var bufferDesc = new BufferDesc
                {
                    ByteWidth = (uint)byteWidth,
                    Usage = Usage.Default,
                    BindFlags = (uint)bindFlag,
                    CPUAccessFlags = (uint)CpuAccessFlag.None,
                    MiscFlags = (uint)ResourceMiscFlag.None,
                    StructureByteStride = (uint)structureByteStride,
                };


                unsafe
                {
                    fixed (T* pData = data)
                    {
                        var subresourceData = new SubresourceData
                        {
                            PSysMem = pData
                        };

                        CrashHandler.CheckForError(device.CreateBuffer(in bufferDesc, in subresourceData, ref buffer), "Failed buffer creation");
                    }


                }


                return buffer;

            }


            public static ComPtr<ID3D11Buffer> Create<T>(ComPtr<ID3D11Device> device, ref T data, int byteWidth, BindFlag bindFlag, int structureByteStride) where T : unmanaged
            {
                ComPtr<ID3D11Buffer> buffer = default;

                var bufferDesc = new BufferDesc
                {
                    ByteWidth = (uint)byteWidth,
                    Usage = Usage.Default,
                    BindFlags = (uint)bindFlag,
                    CPUAccessFlags = (uint)CpuAccessFlag.None,
                    MiscFlags = (uint)ResourceMiscFlag.None,
                    StructureByteStride = (uint)structureByteStride,
                };



                unsafe
                {
                    fixed (T* pData = &data)
                    {
                        var subresourceData = new SubresourceData
                        {
                            PSysMem = pData
                        };

                        CrashHandler.CheckForError(device.CreateBuffer(in bufferDesc, in subresourceData, ref buffer), "Failed buffer creation");
                    }


                }


                return buffer;

            }
        }


        public class RenderTargetView
        {

        }
    }
}
