namespace Manssiere.Core.Graphics.VertexArayUtility.Buffers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Collections;
    using OpenTK.Graphics.OpenGL;

    // TODO - Support deleting the local cache of data and getting from OpenGL when needed
    public abstract class BufferObject<T> : IEnumerable, IDisposable where T : struct
    {
        private int _id;
        private readonly int _strideInBytes = Marshal.SizeOf(typeof(T));
        private bool _capacityChanged = true;

        internal BufferObject() : this(0, BufferTarget.ArrayBuffer) { }
        protected BufferObject(int initialCapacity) : this(initialCapacity, BufferTarget.ArrayBuffer) { }
        protected BufferObject(BufferTarget target) : this(0, target) { }

        protected BufferObject(int initialCapacity, BufferTarget bufferTarget)
        {
            UsageHint = BufferUsageHint.StaticDraw;
            if (initialCapacity > 0) Buffer = new T[initialCapacity];
            Target = bufferTarget;
        }

        ~BufferObject() { Dispose(false); }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If disposing, dispose all managed and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources

                // Unhook events
                NeedsUpdate = null;

                if (_id != 0)
                {
                    GL.DeleteBuffers(1, ref _id);
                    _id = 0;
                }
            }

            // Dispose of unmanaged resources

            // Set fields to null
            Count = 0;
            Buffer = null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator() { for (int i = 0; i < Count; i++) yield return Buffer[i]; }

        /// <summary>
        /// Gets the stride in bytes.
        /// </summary>
        /// <value>The stride in bytes.</value>
        public int StrideInBytes { get { return _strideInBytes; } }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity
        {
            get
            {
                if (Buffer == null) return 0;
                return Buffer.Length;
            }
            set
            {
                Debug.Assert(value >= 0);

                if (Buffer == null)
                {
                    Debug.Assert(Count == 0);
                    if (value > 0)
                    {
                        Buffer = new T[value];
                    }
                    return;
                }

                int newCapacity = Math.Max(value, Count);
                if (newCapacity <= 0)
                {
                    Buffer = null;
                    return;
                }

                if (newCapacity == Buffer.Length) return;

                T[] tempBuffer = new T[newCapacity];

                if (Count != 0)
                {
                    if (tempBuffer.Length > Buffer.Length)
                    {
                        Buffer.CopyTo(tempBuffer, 0);
                    }
                    else
                    {
                        // OPTIMIZATION : Pin and MemCopy?
                        for (int i = 0; i < Count; i++)
                        {
                            tempBuffer[i] = Buffer[i];
                        }
                    }
                }

                Buffer = tempBuffer;

                _capacityChanged = true;
            }
        }

        public void EnsureCapacity(int capacity)
        {
            if( Capacity >= capacity ) return;
            int newCapacity = capacity;
            if (GrowCapacity > 0)
            {
                do
                {
                    newCapacity += GrowCapacity;
                }
                while (newCapacity < capacity);
            }
            else
            {
                do
                {
                    newCapacity += ((newCapacity + 1) / 2);
                }
                while (newCapacity < capacity);
            }
            Capacity = newCapacity;
        }

        public int GrowCapacity { get; set; }

        public int Count { get; private set; }

        public T[] Buffer { get; private set; }

        public void SetBuffer(T[] buffer, int count)
        {
            _capacityChanged = (this.Buffer.Length != buffer.Length);
            this.Buffer = buffer;
            this.Count = count;
            Dirty = true;
        }

        public T this[int index]
        {
            get { return Buffer[index]; }
            set
            {
                Buffer[index] = value;
                Dirty = true;
            }
        }

        public BufferTarget Target { get; set; }

        public BufferUsageHint UsageHint { get; set; }

        public void Bind()
        {
            if (_id == 0) GL.GenBuffers(1, out _id);
            GL.BindBuffer(Target, _id);
        }

        public int Id { get { return _id; } }

        public bool Dirty
        {
            get { return _dirty; }
            set
            {
                bool wasDirty = _dirty;
                _dirty = value;
                if (!wasDirty && _dirty && NeedsUpdate != null)
                {
                    NeedsUpdate();
                }
            }
        }
        internal delegate void VoidDelegate();
        internal event VoidDelegate NeedsUpdate;
        bool _dirty = true;

        public void Update()
        {
            if (!Dirty) return;
            Dirty = false;

            if (Buffer == null) return;

            Bind();

            if (_capacityChanged)
            {
                _capacityChanged = false;
                GL.BufferData(Target, (IntPtr)(Buffer.Length * StrideInBytes), Buffer, UsageHint);
            }
            else
            {
                GL.BufferSubData(Target, (IntPtr)0, (IntPtr)(Count * StrideInBytes), Buffer);
            }
        }

        public void Add(T value)
        {
            EnsureCapacity(Count + 1);
            Buffer[Count] = value;
            Count++;
            Dirty = true;
        }

        public void Clear()
        {
            Count = 0;
            Dirty = true;
        }

        public void RemoveLast(int count)
        {
            this.Count -= count;
            if (this.Count < 0) this.Count = 0;
        }
    }
}


