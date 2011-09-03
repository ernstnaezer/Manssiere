namespace Manssiere.Core.Graphics.VertexArayUtility.Buffers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using OpenTK.Graphics.OpenGL;

    // TODO Support Primitive Restart...  This would go into the ElementBuffer.

    public class ElementBuffer : ElementBufferObject<uint> { }

    public class ElementBufferObject<T> : BufferObject<T> where T : struct
    {
        static DrawElementsType DefaultDrawElementsType
        {
            get
            {
                switch (Marshal.SizeOf(typeof(T))) // Bytes
                {
                    case 1: return DrawElementsType.UnsignedByte;
                    case 2: return DrawElementsType.UnsignedShort;
                    case 4: return DrawElementsType.UnsignedInt;
                    default: Debug.Assert(false); return DrawElementsType.UnsignedInt;
                }
            }
        }

        public ElementBufferObject()
        {
            this.Target = BufferTarget.ElementArrayBuffer;
            drawElementsType = DefaultDrawElementsType;
        }

        public ElementBufferObject(int initialCapacity)
            : base(initialCapacity)
        {
            this.Target = BufferTarget.ElementArrayBuffer;
            drawElementsType = DefaultDrawElementsType;
        }

        public ElementBufferObject(DrawElementsType drawElementsType)
        {
            this.Target = BufferTarget.ElementArrayBuffer;
            this.drawElementsType = drawElementsType;
        }

        public ElementBufferObject(DrawElementsType drawElementsType, int initialCapacity)
            : base(initialCapacity)
        {
            this.Target = BufferTarget.ElementArrayBuffer;
            this.drawElementsType = drawElementsType;
        }

        readonly DrawElementsType drawElementsType;

        public void Enable()
        {
            if (Count == 0) return;
            Update();
            Bind();
        }

        public void DrawElements(BeginMode beginMode)
        {
            GL.DrawElements(beginMode, Count, drawElementsType, IntPtr.Zero);
        }

        public void DrawElementsInstanced(BeginMode beginMode, int instances)
        {
            GL.DrawElementsInstanced(beginMode, Count, drawElementsType, IntPtr.Zero, instances);
        }
    }
}


