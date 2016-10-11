using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ServiceLoader
{
    /// <summary>
    /// 远程加载器
    /// </summary>
    public class RemoteLoader : MarshalByRefObject
    {
        private Assembly _assembly;

        public void LoadAssembly(string assemblyFile)
        {
            try
            {
                _assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetInstance(string typeName)
        {
            if (_assembly == null) return null;
            var type = _assembly.GetType(typeName);
            if (type == null) return null;
            return Activator.CreateInstance(type);
        }

        public void ExecuteMothod(string typeName, string methodName)
        {
            if (_assembly == null) return;
            var type = _assembly.GetType(typeName);
            var obj = Activator.CreateInstance(type);
            Expression<Action> lambda = Expression.Lambda<Action>(Expression.Call(Expression.Constant(obj), type.GetMethod(methodName)), null);
            lambda.Compile()();
        }
    }
}
