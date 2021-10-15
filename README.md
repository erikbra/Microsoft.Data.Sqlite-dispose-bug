# Microsoft.Data.Sqlite-dispose-bug
Demonstrates a bug in Microsoft.Data.Sqlite v 6.0 rc1 onwards.

Reported here: https://github.com/dotnet/efcore/issues/26369


## Description

There seems to be a bug in Microsoft.Data.Sqlite, v 6.0, from rc1 onwards, which makes the database file remain locked after the SqliteConnection is closed, but only on Windows. It works up until preview7 (of the versions published on Nuget.org).

This project uses conditional includes, and you can control which version of Microsoft.Data.Sqlite you want to use by using the `-p:Ver=<rc1,rc2,preview7>` on the command line.

## To demonstrate that it works on previous versions
Run tests without parameters (defaults to latest stable version, 5.11):

```
❯ dotnet test
  Determining projects to restore...
  All projects are up-to-date for restore.
  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview
[C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\SimpleSqliteDisposeTest.csproj]
  SimpleSqliteDisposeTest -> C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\bin\Debug\net6.0\SimpleSqliteDisposeTest.dll
Test run for C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\bin\Debug\net6.0\SimpleSqliteDisposeTest.dll (.NETCoreApp,Version=v6.0)
Microsoft (R) Test Execution Command Line Tool Version 17.0.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
Deleting db file testdb.db to start fresh
Creating connection with connectionString Data Source=testdb.db
Opening connection with connectionString Data Source=testdb.db
Using cmd = conn.createCommand
Disposing Microsoft.Data.Sqlite.SqliteCommand
After using statement - cmd should have been disposed.
Disposing Microsoft.Data.Sqlite.SqliteConnection
After using statement - conn should have been disposed.
Calling File.Delete(testdb.db)

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: 79 ms - SimpleSqliteDisposeTest.dll (net6.0)
```

Or, with preview 7:
```
❯ dotnet test -p:Ver=preview7
  Determining projects to restore...
  Restored C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\SimpleSqliteDisposeTest.csproj (in 1,1 sec).
  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview
[C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\SimpleSqliteDisposeTest.csproj]
  SimpleSqliteDisposeTest -> C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\bin\Debug\net6.0\SimpleSqliteDisposeTest.dll
Test run for C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\bin\Debug\net6.0\SimpleSqliteDisposeTest.dll (.NETCoreApp,Version=v6.0)
Microsoft (R) Test Execution Command Line Tool Version 17.0.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
Deleting db file testdb.db to start fresh
Creating connection with connectionString Data Source=testdb.db
Opening connection with connectionString Data Source=testdb.db
Using cmd = conn.createCommand
Disposing Microsoft.Data.Sqlite.SqliteCommand
After using statement - cmd should have been disposed.
Disposing Microsoft.Data.Sqlite.SqliteConnection
After using statement - conn should have been disposed.
Calling File.Delete(testdb.db)

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: 97 ms - SimpleSqliteDisposeTest.dll (net6.0)
```

## To demonstrate problem:

Run the tests with parameter `-p:Ver=rc1` or `-p:Ver=rc2`, on _Windows_:

```
❯ dotnet test -p:Ver=rc2
  Determining projects to restore...
  Restored C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\SimpleSqliteDisposeTest.csproj (in 362 ms).
  You are using a preview version of .NET. See: https://aka.ms/dotnet-core-preview
[C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\SimpleSqliteDisposeTest.csproj]
  SimpleSqliteDisposeTest -> C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\bin\Debug\net6.0\SimpleSqliteDisposeTest.dll
Test run for C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\bin\Debug\net6.0\SimpleSqliteDisposeTest.dll (.NETCoreApp,Version=v6.0)
Microsoft (R) Test Execution Command Line Tool Version 17.0.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.
Deleting db file testdb.db to start fresh
Creating connection with connectionString Data Source=testdb.db
Opening connection with connectionString Data Source=testdb.db
Using cmd = conn.createCommand
Disposing Microsoft.Data.Sqlite.SqliteCommand
After using statement - cmd should have been disposed.
Disposing Microsoft.Data.Sqlite.SqliteConnection
After using statement - conn should have been disposed.
Calling File.Delete(testdb.db)
  Failed PureSqliteTest [113 ms]
  Error Message:
   System.IO.IOException : The process cannot access the file 'C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\bin\Debug\net6.0\testdb.db' because it is being used by another process.
  Stack Trace:
     at System.IO.FileSystem.DeleteFile(String fullPath)
   at System.IO.File.Delete(String path)
   at SimpleSqliteDisposeTest.SqliteDisposeTest.PureSqliteTest() in C:\Users\erikb\Source\Repos\Microsoft.Data.Sqlite-dispose-bug\SimpleSqliteDisposeTest\SqliteDisposeTest.cs:line 50
   at NUnit.Framework.Internal.TaskAwaitAdapter.GenericAdapter`1.BlockUntilCompleted()
   at NUnit.Framework.Internal.MessagePumpStrategy.NoMessagePumpStrategy.WaitForCompletion(AwaitAdapter awaiter)
   at NUnit.Framework.Internal.AsyncToSyncAdapter.Await(Func`1 invoke)
   at NUnit.Framework.Internal.Commands.TestMethodCommand.RunTestMethod(TestExecutionContext context)
   at NUnit.Framework.Internal.Commands.TestMethodCommand.Execute(TestExecutionContext context)
   at NUnit.Framework.Internal.Execution.SimpleWorkItem.<>c__DisplayClass4_0.<PerformWork>b__0()
   at NUnit.Framework.Internal.ContextUtils.<>c__DisplayClass1_0`1.<DoIsolated>b__0(Object _)
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
--- End of stack trace from previous location ---
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
   at NUnit.Framework.Internal.ContextUtils.DoIsolated(ContextCallback callback, Object state)
   at NUnit.Framework.Internal.ContextUtils.DoIsolated[T](Func`1 func)
   at NUnit.Framework.Internal.Execution.SimpleWorkItem.PerformWork()

Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1, Duration: 112 ms - SimpleSqliteDisposeTest.dll (net6.0)
```

