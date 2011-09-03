namespace Manssiere.Core.Graphics.VertexArayUtility.Buffers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    public class TexCoordBuffer : TexCoordBufferObject<Vector2> { }

    public class TexCoordBufferObject<T> : BufferObject<T> where T : struct
    {
        #region Static Default Value Properties

        static int TexCoordPointerTypeSize(TexCoordPointerType texCoordPointerType)
        {
            switch (texCoordPointerType)
            {
                case TexCoordPointerType.Short: return sizeof(short);
                case TexCoordPointerType.Int: return sizeof(int);
                case TexCoordPointerType.HalfFloat: return Half.SizeInBytes;
                case TexCoordPointerType.Float: return sizeof(float);
                case TexCoordPointerType.Double: return sizeof(double);
                default: Debug.Assert(false); return 0;
            }
        }

        static TexCoordPointerType DefaultTexCoordPointerType
        {
            get
            {
                switch (Marshal.SizeOf(typeof(T))) // Bytes
                {
                    case 2: return TexCoordPointerType.Short; // 1 * Short
                    case 4: return TexCoordPointerType.Short; // 2 * Short
                    case 8: return TexCoordPointerType.Float; // 2 * Float
                    case 16: return TexCoordPointerType.Double; // 2 * Double
                    case 32: return TexCoordPointerType.Double; // 4 * Double
                    default: Debug.Assert(false); return TexCoordPointerType.Float;
                }
            }
        }

        #endregion

        public TexCoordBufferObject() : this(0) { }
        public TexCoordBufferObject(int initialCapacity) : this (DefaultTexCoordPointerType, initialCapacity) { }
        public TexCoordBufferObject(TexCoordPointerType texCoordPointerType) : this(texCoordPointerType, 0) { }
        public TexCoordBufferObject(TexCoordPointerType texCoordPointerType, int initialCapacity) : base(initialCapacity)
        {
            _texCoordPointerType = texCoordPointerType;
            _componentCount = StrideInBytes / TexCoordPointerTypeSize(texCoordPointerType);
        }

        private readonly TexCoordPointerType _texCoordPointerType;
        private readonly int _componentCount;
        private TextureUnit _textureUnit = TextureUnit.Texture0;

        /// <summary>
        /// Gets or sets the texture unit.
        /// </summary>
        /// <value>The texture unit.</value>
        public TextureUnit TextureUnit
        {
            get { return _textureUnit; }
            set { _textureUnit = value; }
        }

        /// <summary>
        /// Enables this instance.
        /// </summary>
        public void Enable()
        {
            GL.ClientActiveTexture(TextureUnit);

            if (Count == 0)
            {
                GL.DisableClientState(EnableCap.TextureCoordArray);
                return;
            }

            Update();
            Bind();

            GL.EnableClientState(EnableCap.TextureCoordArray);
            GL.TexCoordPointer(_componentCount, _texCoordPointerType, StrideInBytes, IntPtr.Zero);
        }
    }
}


