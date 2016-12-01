using System.Diagnostics;
using NHibernate;
using NHibernate.SqlCommand;

namespace IntergrationTests
{
    public class SqlStatementInterceptor : EmptyInterceptor
    {
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            Trace.WriteLine(sql.ToString());

            return sql;
        }
    }
}
