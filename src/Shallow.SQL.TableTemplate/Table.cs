using Shallow.SQL;
using Shallow.SQL.Attributes;
using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5) using System.Linq;$endif$
using System.Text;

namespace $rootnamespace$
{
	internal class $safeitemrootname$ : ModelTable<$safeitemrootname$>
    {
        internal static readonly ModelQuery<$safeitemrootname$> Query = ModelQuery.GetInstanceOf <$safeitemrootname$>();

        /// Here goes table columns as properties. Obligatory to have { get; set; }, you can change the access modifiers as you wish
        // public string ColumnName { get; set; }

        /// Also can define relations as get-only properties or methods. Types of relations: belongsTo(), hasOne(), hasMany(), belongsToMany()
        // public TableClass[] Users => belongsTo(TableClass.Query);
    }
}
