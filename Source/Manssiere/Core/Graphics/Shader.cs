namespace Manssiere.Core.Graphics
{
    using System;
    using System.IO;
    using Core;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;

    /// <summary>
    /// OpenGl shader wrapper.
    /// </summary>
    public class Shader : IDisposable
    {
        private readonly int _programHandle;

        /// <summary>
        /// Create a new shader program.
        /// </summary>
        private Shader()
        {
            _programHandle = GL.CreateProgram();
        }

        /// <summary>
        /// Load a new shader
        /// </summary>
        /// <param name="vertexShader">The vertexshader.</param>
        /// <param name="fragmentShader">The fragmentshader</param>
        public static Shader Load(StringReader vertexShader, StringReader fragmentShader)
        {
            var shader = new Shader();

            string vertexShaderCode = vertexShader.ReadToEnd();
            string fragmentShaderCode = fragmentShader.ReadToEnd();

            if (string.IsNullOrEmpty(vertexShaderCode) && string.IsNullOrEmpty(fragmentShaderCode))
            {
                throw new InvalidOperationException("Both vertex and fragement shader code are empty.");
            }

            if (string.IsNullOrEmpty(vertexShaderCode) == false)
            {
                shader.AttachShaderCode(vertexShaderCode, ShaderType.VertexShader);
            }
            if (string.IsNullOrEmpty(fragmentShaderCode) == false)
            {
                shader.AttachShaderCode(fragmentShaderCode, ShaderType.FragmentShader);
            }

            shader.Build();

            return shader;
        }

        /// <summary>
        /// Load a new shader
        /// </summary>
        /// <param name="vertexShaderCode">The vertexshader code.</param>
        /// <param name="fragmentShaderCode">The fragmentshader code</param>
        public static Shader Load(string vertexShaderCode, string fragmentShaderCode)
        {
            return Load(new StringReader(vertexShaderCode), new StringReader(fragmentShaderCode));
        }

        #region IDisposable Members

        ///<summary>
        ///Release the loaded shader
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_programHandle > 0)
                GL.DeleteProgram(_programHandle);
        }

        #endregion

        /// <summary>
        /// Helper to use the framebuffer in a using construction.
        /// </summary>
        /// <returns>The framebuffer dispose helper.</returns>
        public IDisposable PushShader()
        {
            InternalEnableShader();
            return new DisposableAction(InternalDisableShader);
        }

        /// <summary>
        /// (Re)link the shader program
        /// </summary>
        private void Build()
        {
            int status;

            GL.LinkProgram(_programHandle);
            GL.GetProgram(_programHandle, ProgramParameter.LinkStatus, out status);
            if (status != 1)
            {
                string info;
                GL.GetShaderInfoLog(_programHandle, out info);
                throw new ApplicationException("Error during shader program setup." + info);
            }

            InternalDisableShader();
        }

        /// <summary>
        /// Enable this shader.
        /// </summary>
        private void InternalEnableShader()
        {
            GL.UseProgram(_programHandle);
        }

        /// <summary>
        /// Disable this shader.
        /// </summary>
        private static void InternalDisableShader()
        {
            GL.UseProgram(0);
        }

        /// <summary>
        /// Attach the code to the shader program.
        /// </summary>
        /// <param name="shaderCode">The shader code.</param>
        /// <param name="shaderType">The code type.</param>
        private void AttachShaderCode(string shaderCode, ShaderType shaderType)
        {
            int shaderHandle = GL.CreateShader(shaderType);

            int status;

            // load and compile the shader code
            GL.ShaderSource(shaderHandle, shaderCode);
            GL.CompileShader(shaderHandle);
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out status);
            if (status != 1)
            {
                string log;
                GL.GetShaderInfoLog(shaderHandle, out log);
                throw new ApplicationException("Can not compile the shader code" + log);
            }

            // attach to the program
            GL.AttachShader(_programHandle, shaderHandle);
            // flag the shader objects for deletion. They won't be deleted until the program is deleted.
            GL.DeleteShader(shaderHandle);
        }

        private int GetUniformLocation(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            int id = GL.GetUniformLocation(_programHandle, name);
            if (id == -1)
            {
                throw new ApplicationException(string.Format("Can not locate uniform with name {0}", name));
            }

            return id;
        }

        /// <summary>
        /// Set a Single value on a shader variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value.</param>
        public void SetUniform(string name, Single value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        /// <summary>
        /// Set a Single cast to a Single value on a shader variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value.</param>
        public void SetUniform(string name, double value)
        {
            GL.Uniform1(GetUniformLocation(name), (Single) value);
        }

        /// <summary>
        /// Set a Vector3D value on a shader variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value.</param>
        public void SetUniform(string name, Vector3 value)
        {
            GL.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Set a Vector4D value on a shader variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value.</param>
        public void SetUniform(string name, Vector4 value)
        {
            GL.Uniform4(GetUniformLocation(name), value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Set a int 1 value on a shader variable.
        /// </summary>
        /// <remarks>When binding textures, bind to the channel not the texture handle!</remarks>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value.</param>
        public void SetUniform(string name, int value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }
    }
}