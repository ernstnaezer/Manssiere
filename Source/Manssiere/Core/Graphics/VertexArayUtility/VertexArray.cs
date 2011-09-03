namespace Manssiere.Core.Graphics.VertexArayUtility
{
    using System.Linq;
    using Buffers;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections.Generic;
 
    public class VertexArray : VertexArrayObject<Vector3, Vector3, Vector2, uint, uint, uint>
    {
        public VertexArray() { }
        public VertexArray(BeginMode geometry) { Geometry = geometry; }
    }

    public class VertexArrayObject<TVertexStruct, TNormalStruct, TExCoordStruct, TElementStruct, TColorStruct, TAttribStruct> : IDisposable
        where TVertexStruct : struct
        where TNormalStruct : struct
        where TExCoordStruct : struct
        where TElementStruct : struct
        where TColorStruct : struct
        where TAttribStruct : struct
    {
    //    static bool supportsFrameBufferObjects = GL.SupportsFunction("glBufferData");
      //  static bool supportsVertexArrayObjects = GL.SupportsFunction("glBindVertexArray");

        ~VertexArrayObject() { Dispose(false); }

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

                foreach (TexCoordBufferObject<TExCoordStruct> texCoords in _texCoordsArray)
                {
                    if (texCoords == null) continue;
                    texCoords.Dispose();
                }

                if (Attributes != null) Attributes.Dispose();
                if (Colors != null) Colors.Dispose();
                if (SecondaryColors != null) SecondaryColors.Dispose();
                if (Normals != null) Normals.Dispose();
                if (Vertices != null) Vertices.Dispose();

                foreach (ElementBufferObject<TElementStruct> elementBuffer in _elementBuffersArray)
                {
                    elementBuffer.Dispose();
                }

                //if (vertexArrayObjectID != 0)
                //{
                //    GL.DeleteVertexArrays(1, ref vertexArrayObjectID);
                //    vertexArrayObjectID = 0;
                //}
            }

            // Dispose of unmanaged resources

            for (int i = 0; i < _texCoordsArray.Length; i++)
            {
                _texCoordsArray[i] = null;
            }

            for (int i = 0; i < _elementBuffersArray.Count; i++)
            {
                _elementBuffersArray[i] = null;
            }

            // Set fields to null
            Attributes = null;
            Colors = null;
            SecondaryColors = null;
            Normals = null;
            Vertices = null;
        }

        public VertexAttribBufferObject<TAttribStruct> Attributes
        {
            get { return _vertexAttribs; }
            set
            {
                if( _vertexAttribs == value ) return;
                if (_vertexAttribs != null)
                {
                    _vertexAttribs.NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _vertexAttribs = value;
                if (_vertexAttribs != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _vertexAttribs.Dirty;
                    _vertexAttribs.NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        VertexAttribBufferObject<TAttribStruct> _vertexAttribs;

        public ColorBufferObject<TColorStruct> Colors
        {
            get { return _colors; }
            set
            {
                if (_colors == value) return;
                if (_colors != null)
                {
                    _colors.NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _colors = value;
                if (_colors != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _colors.Dirty;
                    _colors.NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        ColorBufferObject<TColorStruct> _colors;

        public ColorBufferObject<TColorStruct> SecondaryColors
        {
            get { return _secondaryColors; }
            set
            {
                if (_secondaryColors == value) return;
                if (_secondaryColors != null)
                {
                    _secondaryColors.NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _secondaryColors = value;
                if (_secondaryColors != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _secondaryColors.Dirty;
                    _secondaryColors.NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        ColorBufferObject<TColorStruct> _secondaryColors;

        public TexCoordBufferObject<TExCoordStruct> TexCoords
        {
            get { return _texCoordsArray[0]; }
            set
            {
                if (_texCoordsArray[0] == value) return;
                if (_texCoordsArray[0] != null)
                {
                    _texCoordsArray[0].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[0] = value;
                if (_texCoordsArray[0] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[0].Dirty;
                    _texCoordsArray[0].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords1
        {
            get { return _texCoordsArray[1]; }
            set
            {
                if (_texCoordsArray[1] == value) return;
                if (_texCoordsArray[1] != null)
                {
                    _texCoordsArray[1].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[1] = value;
                if (_texCoordsArray[1] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[1].Dirty;
                    _texCoordsArray[1].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords2
        {
            get { return _texCoordsArray[2]; }
            set
            {
                if (_texCoordsArray[2] == value) return;
                if (_texCoordsArray[2] != null)
                {
                    _texCoordsArray[2].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[2] = value;
                if (_texCoordsArray[2] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[2].Dirty;
                    _texCoordsArray[2].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords3
        {
            get { return _texCoordsArray[3]; }
            set
            {
                if (_texCoordsArray[3] == value) return;
                if (_texCoordsArray[3] != null)
                {
                    _texCoordsArray[3].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[3] = value;
                if (_texCoordsArray[3] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[3].Dirty;
                    _texCoordsArray[3].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords4
        {
            get { return _texCoordsArray[4]; }
            set
            {
                if (_texCoordsArray[4] == value) return;
                if (_texCoordsArray[4] != null)
                {
                    _texCoordsArray[4].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[4] = value;
                if (_texCoordsArray[4] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[4].Dirty;
                    _texCoordsArray[4].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords5
        {
            get { return _texCoordsArray[5]; }
            set
            {
                if (_texCoordsArray[5] == value) return;
                if (_texCoordsArray[5] != null)
                {
                    _texCoordsArray[5].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[5] = value;
                if (_texCoordsArray[5] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[5].Dirty;
                    _texCoordsArray[5].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords6
        {
            get { return _texCoordsArray[6]; }
            set
            {
                if (_texCoordsArray[6] == value) return;
                if (_texCoordsArray[6] != null)
                {
                    _texCoordsArray[6].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[6] = value;
                if (_texCoordsArray[6] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[6].Dirty;
                    _texCoordsArray[6].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords7
        {
            get { return _texCoordsArray[7]; }
            set
            {
                if (_texCoordsArray[7] == value) return;
                if (_texCoordsArray[7] != null)
                {
                    _texCoordsArray[7].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[7] = value;
                if (_texCoordsArray[7] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[7].Dirty;
                    _texCoordsArray[7].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords8
        {
            get { return _texCoordsArray[8]; }
            set
            {
                if (_texCoordsArray[8] == value) return;
                if (_texCoordsArray[8] != null)
                {
                    _texCoordsArray[8].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[8] = value;
                if (_texCoordsArray[8] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[8].Dirty;
                    _texCoordsArray[8].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords9
        {
            get { return _texCoordsArray[9]; }
            set
            {
                if (_texCoordsArray[9] == value) return;
                if (_texCoordsArray[9] != null)
                {
                    _texCoordsArray[9].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[9] = value;
                if (_texCoordsArray[9] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[9].Dirty;
                    _texCoordsArray[9].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords10
        {
            get { return _texCoordsArray[10]; }
            set
            {
                if (_texCoordsArray[10] == value) return;
                if (_texCoordsArray[10] != null)
                {
                    _texCoordsArray[10].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[10] = value;
                if (_texCoordsArray[10] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[10].Dirty;
                    _texCoordsArray[10].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords11
        {
            get { return _texCoordsArray[11]; }
            set
            {
                if (_texCoordsArray[11] == value) return;
                if (_texCoordsArray[11] != null)
                {
                    _texCoordsArray[11].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[11] = value;
                if (_texCoordsArray[11] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[11].Dirty;
                    _texCoordsArray[11].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords12
        {
            get { return _texCoordsArray[12]; }
            set
            {
                if (_texCoordsArray[12] == value) return;
                if (_texCoordsArray[12] != null)
                {
                    _texCoordsArray[12].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[12] = value;
                if (_texCoordsArray[12] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[12].Dirty;
                    _texCoordsArray[12].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords13
        {
            get { return _texCoordsArray[13]; }
            set
            {
                if (_texCoordsArray[13] == value) return;
                if (_texCoordsArray[13] != null)
                {
                    _texCoordsArray[13].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[13] = value;
                if (_texCoordsArray[13] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[13].Dirty;
                    _texCoordsArray[13].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords14
        {
            get { return _texCoordsArray[14]; }
            set
            {
                if (_texCoordsArray[14] == value) return;
                if (_texCoordsArray[14] != null)
                {
                    _texCoordsArray[14].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[14] = value;
                if (_texCoordsArray[14] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[14].Dirty;
                    _texCoordsArray[14].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords15
        {
            get { return _texCoordsArray[15]; }
            set
            {
                if (_texCoordsArray[15] == value) return;
                if (_texCoordsArray[15] != null)
                {
                    _texCoordsArray[15].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[15] = value;
                if (_texCoordsArray[15] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[15].Dirty;
                    _texCoordsArray[15].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords16
        {
            get { return _texCoordsArray[16]; }
            set
            {
                if (_texCoordsArray[16] == value) return;
                if (_texCoordsArray[16] != null)
                {
                    _texCoordsArray[16].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[16] = value;
                if (_texCoordsArray[16] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[16].Dirty;
                    _texCoordsArray[16].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords17
        {
            get { return _texCoordsArray[17]; }
            set
            {
                if (_texCoordsArray[17] == value) return;
                if (_texCoordsArray[17] != null)
                {
                    _texCoordsArray[17].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[17] = value;
                if (_texCoordsArray[17] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[17].Dirty;
                    _texCoordsArray[17].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords18
        {
            get { return _texCoordsArray[18]; }
            set
            {
                if (_texCoordsArray[18] == value) return;
                if (_texCoordsArray[18] != null)
                {
                    _texCoordsArray[18].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[18] = value;
                if (_texCoordsArray[18] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[18].Dirty;
                    _texCoordsArray[18].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords19
        {
            get { return _texCoordsArray[19]; }
            set
            {
                if (_texCoordsArray[19] == value) return;
                if (_texCoordsArray[19] != null)
                {
                    _texCoordsArray[19].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[19] = value;
                if (_texCoordsArray[19] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[19].Dirty;
                    _texCoordsArray[19].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords20
        {
            get { return _texCoordsArray[20]; }
            set
            {
                if (_texCoordsArray[20] == value) return;
                if (_texCoordsArray[20] != null)
                {
                    _texCoordsArray[20].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[20] = value;
                if (_texCoordsArray[20] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[20].Dirty;
                    _texCoordsArray[20].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords21
        {
            get { return _texCoordsArray[21]; }
            set
            {
                if (_texCoordsArray[21] == value) return;
                if (_texCoordsArray[21] != null)
                {
                    _texCoordsArray[21].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[21] = value;
                if (_texCoordsArray[21] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[21].Dirty;
                    _texCoordsArray[21].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords22
        {
            get { return _texCoordsArray[22]; }
            set
            {
                if (_texCoordsArray[22] == value) return;
                if (_texCoordsArray[22] != null)
                {
                    _texCoordsArray[22].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[22] = value;
                if (_texCoordsArray[22] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[22].Dirty;
                    _texCoordsArray[22].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords23
        {
            get { return _texCoordsArray[23]; }
            set
            {
                if (_texCoordsArray[23] == value) return;
                if (_texCoordsArray[23] != null)
                {
                    _texCoordsArray[23].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[23] = value;
                if (_texCoordsArray[23] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[23].Dirty;
                    _texCoordsArray[23].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords24
        {
            get { return _texCoordsArray[24]; }
            set
            {
                if (_texCoordsArray[24] == value) return;
                if (_texCoordsArray[24] != null)
                {
                    _texCoordsArray[24].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[24] = value;
                if (_texCoordsArray[24] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[24].Dirty;
                    _texCoordsArray[24].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords25
        {
            get { return _texCoordsArray[25]; }
            set
            {
                if (_texCoordsArray[25] == value) return;
                if (_texCoordsArray[25] != null)
                {
                    _texCoordsArray[25].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[25] = value;
                if (_texCoordsArray[25] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[25].Dirty;
                    _texCoordsArray[25].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords26
        {
            get { return _texCoordsArray[26]; }
            set
            {
                if (_texCoordsArray[26] == value) return;
                if (_texCoordsArray[26] != null)
                {
                    _texCoordsArray[26].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[26] = value;
                if (_texCoordsArray[26] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[26].Dirty;
                    _texCoordsArray[26].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords27
        {
            get { return _texCoordsArray[27]; }
            set
            {
                if (_texCoordsArray[27] == value) return;
                if (_texCoordsArray[27] != null)
                {
                    _texCoordsArray[27].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[27] = value;
                if (_texCoordsArray[27] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[27].Dirty;
                    _texCoordsArray[27].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords28
        {
            get { return _texCoordsArray[28]; }
            set
            {
                if (_texCoordsArray[28] == value) return;
                if (_texCoordsArray[28] != null)
                {
                    _texCoordsArray[28].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[28] = value;
                if (_texCoordsArray[28] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[28].Dirty;
                    _texCoordsArray[28].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords29
        {
            get { return _texCoordsArray[29]; }
            set
            {
                if (_texCoordsArray[29] == value) return;
                if (_texCoordsArray[29] != null)
                {
                    _texCoordsArray[29].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[29] = value;
                if (_texCoordsArray[29] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[29].Dirty;
                    _texCoordsArray[29].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords30
        {
            get { return _texCoordsArray[30]; }
            set
            {
                if (_texCoordsArray[30] == value) return;
                if (_texCoordsArray[30] != null)
                {
                    _texCoordsArray[30].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[30] = value;
                if (_texCoordsArray[30] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[30].Dirty;
                    _texCoordsArray[30].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        public TexCoordBufferObject<TExCoordStruct> TexCoords31
        {
            get { return _texCoordsArray[31]; }
            set
            {
                if (_texCoordsArray[31] == value) return;
                if (_texCoordsArray[31] != null)
                {
                    _texCoordsArray[31].NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _texCoordsArray[31] = value;
                if (_texCoordsArray[31] != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _texCoordsArray[31].Dirty;
                    _texCoordsArray[31].NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }

        readonly TexCoordBufferObject<TExCoordStruct>[] _texCoordsArray = new TexCoordBufferObject<TExCoordStruct>[(int)TextureUnit.Texture31 - (int)TextureUnit.Texture0 + 1];

        public NormalBufferObject<TNormalStruct> Normals
        {
            get { return _normals; }
            set
            {
                if (_normals == value) return;
                if (_normals != null)
                {
                    _normals.NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _normals = value;
                if (_normals != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _normals.Dirty;
                    _normals.NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        NormalBufferObject<TNormalStruct> _normals;

        public VertexBufferObject<TVertexStruct> Vertices
        {
            get { return _vertices; }
            set
            {
                if (_vertices == value) return;
                if (_vertices != null)
                {
                    _vertices.NeedsUpdate -= SetBufferUpdateNeeded;
                }
                _vertices = value;
                if (_vertices != null)
                {
                    if (!_bufferUpdateNeeded) _bufferUpdateNeeded = _vertices.Dirty;
                    _vertices.NeedsUpdate += SetBufferUpdateNeeded;
                }
                _vertexArrayObjectUpdateNeeded = true;
            }
        }
        VertexBufferObject<TVertexStruct> _vertices;

        public ElementBufferObject<TElementStruct> Elements
        {
            get
            {
                if (_elementBuffersArray.Count == 0) return null;
                return _elementBuffersArray[0];
            }
            set
            {
                if (Elements == value) return;
                if (Elements != null)
                {
                    Elements.NeedsUpdate -= SetBufferUpdateNeeded;
                    _elementBuffersArray.RemoveAt(0);
                }
                if (value != null)
                {
                    _elementBuffersArray.Insert(0, value);
                    if (!_bufferUpdateNeeded) if (Elements != null) _bufferUpdateNeeded = Elements.Dirty;
                    if (Elements != null) Elements.NeedsUpdate += SetBufferUpdateNeeded;
                }
            }
        }

        readonly List<ElementBufferObject<TElementStruct>> _elementBuffersArray = new List<ElementBufferObject<TElementStruct>>();

        bool _bufferUpdateNeeded;
        void SetBufferUpdateNeeded() { _bufferUpdateNeeded = true; }

        bool _vertexArrayObjectUpdateNeeded;

        protected void Update()
        {
            if (!_bufferUpdateNeeded)
            {
                return;
            }

            foreach (var texCoords in _texCoordsArray.TakeWhile(texCoords => texCoords != null))
            {
                texCoords.Update();
            }

            if (_colors != null) _colors.Update();
            if (_secondaryColors != null) _secondaryColors.Update();
            if (_normals != null) _normals.Update();
            if (_vertexAttribs != null) _vertexAttribs.Update();
            if (_vertices != null) _vertices.Update();

            foreach (var elementBuffer in _elementBuffersArray)
            {
                elementBuffer.Update();
            }
        }

        public BeginMode Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }
        BeginMode _geometry = BeginMode.Triangles;

        public int Instances { get; set; }

        public VertexArrayObject()
        {
            Instances = 1;
        }

        void SetupBuffers()
        {
            int texCoordIndex = 0;
            foreach (var texCoords in _texCoordsArray.TakeWhile(texCoords => texCoords != null))
            {
                texCoords.TextureUnit = TextureUnit.Texture0 + texCoordIndex;
                texCoords.Enable();
                texCoordIndex++;
            }

            if (_colors != null)
            {
                _colors.IsSecondary = false;
                _colors.Enable();
            }

            if (_secondaryColors != null)
            {
                _secondaryColors.IsSecondary = true;
                _secondaryColors.Enable();
            }

            if (_normals != null) _normals.Enable();
            if (_vertexAttribs != null) _vertexAttribs.Enable();
            if (_vertices != null) _vertices.Enable();
        }

        public void Render()
        {
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
            
            Update();

            if (_vertexArrayObjectUpdateNeeded)
            {
                _vertexArrayObjectUpdateNeeded = false;
                SetupBuffers();
            }
            else
            {
                SetupBuffers();
            }

            if (_elementBuffersArray.Count != 0)
            {
                foreach (var elementBuffer in _elementBuffersArray)
                {
                    elementBuffer.Enable();

                    if (Instances <= 1)
                    {
                        elementBuffer.DrawElements(_geometry);
                    }
                    else
                    {
                        elementBuffer.DrawElementsInstanced(_geometry, Instances);
                    }
                }
            }
            else
            {
                if (Instances <= 1)
                {
                    GL.DrawArrays(_geometry, 0, _vertices.Count);
                }
                else
                {
                    GL.DrawArraysInstanced(_geometry, 0, _vertices.Count, Instances);
                }
            }
            GL.PopClientAttrib();
        }
    }
}


