using System;
using System.Collections.Generic;
using System.Text;

namespace Shallow.SQL
{
    public static class ModelQuery
    {
        private static Dictionary<Type, object> s_models = new Dictionary<Type, object>();

        /// <summary>
        /// Returns a generic ModelQuery&lt;<typeparamref name="U"/>&gt; of the specified ModelTable
        /// </summary>
        /// <typeparam name="U">ModelTable</typeparam>
        /// <returns>ModelQuery&lt;<typeparamref name="U"/>&gt;</returns>
        public static ModelQuery<U> GetInstanceOf<U>() where U : ModelTable<U>
            => instance<U>();

        internal static ModelQuery<U> GetInstance<U>()
            => instance<U>();

        private static ModelQuery<U> instance<U>()
        {
            if (!s_models.ContainsKey(typeof(U)))
                s_models.Add(typeof(U), new ModelQuery<U>());

            return (ModelQuery<U>)s_models[typeof(U)];
        }
    }
}
