# Content dashboard

The dashboard lives at `/admin`. It edits the site's content (projects, services, hero slides, Who We Are, Foundation lists, country pages, the blog, images, settings) and holds the enquiries submitted through the website. Content is stored in PostgreSQL; the public pages read it through a cache that is cleared whenever something is saved.

## What you need

- .NET 8
- PostgreSQL 14 or newer

## Configuration

Set these as environment variables (a double underscore stands for a colon) or in `appsettings.Production.json`. Do not commit real values.

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings__Default` | Npgsql connection string, e.g. `Host=...;Database=oe;Username=...;Password=...` |
| `SuperAdmin__Email`, `SuperAdmin__Password` | Creates the hidden super admin the first time the app starts against an empty user table. Password: at least 12 characters. |
| `Storage__UploadsPath` | Folder for uploaded images. Defaults to `uploads` under the app folder; point it somewhere that survives a redeploy and that the service user can write to. |
| `SiteUrl` | Public site address, used in canonical URLs, the sitemap and the RSS feed. |

On startup the app applies any pending database migrations, creates the roles, and (only into empty tables) loads the content that used to be hard-coded in the page models. Public pages come out byte-for-byte the same as before the move.

## Local development

```bash
createuser --createdb oe && createdb -O oe oe_dev        # then set a password for the role
export SuperAdmin__Email=you@example.com SuperAdmin__Password='a-long-passphrase'
dotnet run --project OptimumEarth.Web
```

`appsettings.Development.json` points at `oe_dev` on localhost. New migrations: `dotnet ef migrations add <Name> -o Data/Migrations` from `OptimumEarth.Web`.

## People and access

- **Super admin** is hidden: it is never listed, counted or searchable, cannot be edited from the screen, and appears in the audit log as "Platform admin". It is the only role that can open Users. To rotate its password: `SuperAdmin__Password='new-long-passphrase' dotnet OptimumEarth.Web.dll --reset-superadmin`.
- **Editor** and **Viewer** are managed under Users. For each person the super admin sets None, View or Edit on ten page areas (Home, Projects, Services, Who we are, Foundation, Blog, Country pages, Inquiries, Media library, Site settings). The role is the ceiling: a Viewer never edits, whatever is ticked. New people start with no pages.
- Access is checked on the server on every request and read from the database each time, so a change or a deactivation applies on the person's next click. Access only controls the dashboard; the public site is unaffected.
- **Inviting**: the app has no mail server, so after inviting someone (or creating a reset link) the screen shows a one-use link, valid for 3 days, to copy and send yourself.
- Five failed sign-ins lock an account for 15 minutes.

## Things worth knowing

- **Country pages**: each page's service cards and project cards are chosen and ordered on the page's edit screen. Projects are capped per page (Site settings, default 3, matching the three-across layout); services have no cap. Project and service forms also have an "Appears on" setting that edits the same placements.
- **Zambia's stand-in cards** ("Project to be confirmed", "Regional experience applies here") are stored as three projects hidden from the Projects page, so they can be edited into real case studies or swapped out.
- **Blog**: a post is live when it is published and its date has arrived, so a post dated in the future is scheduled and appears on its day. Articles are Markdown with raw HTML disabled, and the result is sanitised. The Blog link in the header and footer only appears once a post is live. `/blog/rss.xml` and `/sitemap.xml` are generated from the database.
- **Images**: JPG, PNG, WebP or GIF, up to 5 MB, checked by content as well as extension. An image in use anywhere cannot be deleted. The images shipped in `wwwroot/img/optimum-earth-images` are listed but cannot be deleted from the dashboard.
- **Enquiries** from the Contact page and the short forms are stored and shown under Inquiries (note, status, CSV export, delete for privacy requests). They are deleted automatically after the retention period in Site settings (default 24 months; 0 keeps them).
- **Data Protection keys** are stored in the database, so sessions and form tokens survive a redeploy.

## Backups

Back up the PostgreSQL database and the uploads folder together. The database holds the content, the users and the enquiries.

## Not managed in the dashboard

Fixed copy that lives in the `.cshtml` views is still edited in code: the Foundation page's introductory text, the Who We Are statement and stats, the Services and Projects headings, the Contact page, and the Home page's section headings. `ServicePanelContent` (the Home "what we do" panels) exists as a record but is not used by any page, so it has no screen.
