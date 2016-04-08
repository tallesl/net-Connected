<p align="center">
    <a href="#connected">
        <img alt="logo" src="Assets/logo-200x200.png">
    </a>
</p>

# Connected

[![][build-img]][build]
[![][nuget-img]][nuget]

Issues tests commands to HTTP, RDBMS, Redis and SMTP.

[build]:     https://ci.appveyor.com/project/TallesL/net-connected
[build-img]: https://ci.appveyor.com/api/projects/status/github/tallesl/net-connected?svg=true
[nuget]:     https://www.nuget.org/packages/Connected
[nuget-img]: https://badge.fury.io/nu/Connected.svg

## RDBMS

Issues `SELECT 1` to a database server and checks for any errors.

```cs
using ConnectedLibrary;

if (Connected.Rdbms("ConnectionStringName"))
{
   // Ready to go
}
```

## Redis

Issues a `PING` to a Redis server and checks for a `PONG`.

```cs
using ConnectedLibrary;

if (Connected.Redis("example.org", 6379))
{
   // Ready to go
}
```

## HTTP

Issues a `GET` request to a HTTP server and checks for a status code in the 200 family.

```cs
if (Connected.Redis("http://example.org"))
{
    // Ready to go
}
```

## SMTP

Issues a `HELO` to a SMTP server and checks for a reply code of `200`, `220` or `250`.

```cs
using ConnectedLibrary;

if (Connected.Smtp("example.org", 25))
{
   // Ready to go
}
```
