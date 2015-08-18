# Connection Tester

[![][build-img]][build]
[![][nuget-img]][nuget]

[build]:     https://ci.appveyor.com/project/TallesL/ConnectionTester
[build-img]: https://ci.appveyor.com/api/projects/status/github/tallesl/ConnectionTester

[nuget]:     http://badge.fury.io/nu/ConnectionTester
[nuget-img]: https://badge.fury.io/nu/ConnectionTester.png

Issues tests commands to SMTP, RDBMS and Redis servers.

## SMTP

Issues a [`HELO`] to a SMTP server and checks the [reply code] for a `200` (non standard), `220` or `250`.

[`HELO`]:     https://tools.ietf.org/html/rfc5321#section-3.2
[reply code]: https://tools.ietf.org/html/rfc5321#section-4.2.3

```cs
using ConnectionTests;

if (ConnectionTester.SMTP.IsOk("example.org", 25))
{
   // Ready to go
}
```

## RDBMS

Issues a `SELECT 1` to a SMTP server and checks if it went OK.

```cs
using ConnectionTests;

if (ConnectionTester.RDBMS.IsOk("Data Source=.\SQLEXPRESS;Initial Catalog=MyDatabase;Integrated Security=true"))
{
   // Ready to go
}
```

## Redis

Issues a [`PING`] to a Redis server and checks if it went OK.

[`PING`]: http://redis.io/commands/ping

```cs
using ConnectionTests;

if (ConnectionTester.Redis.IsOk("example.org", 6379))
{
   // Ready to go
}
```
