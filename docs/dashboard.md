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

## Website enquiry email

The super admin manages **Site → Email configuration** (`/admin/email`), also linked from Site settings. Enter a sending mailbox, its Hostinger password and a recipient for each destination (Uganda, Zambia and Foundation). The shared server is `smtp.hostinger.com`, port 465 with SSL/TLS. Save each profile, then use **Send test email** to send to its saved recipient. A blank password keeps the existing credential; changing the sending mailbox requires a password. Passwords are encrypted with ASP.NET Data Protection and never displayed, exported or written to the audit log. Preserve the database's Data Protection key ring along with the mailbox records when backing up or moving the site.

The Contact page selects Foundation for Foundation supporters, otherwise the selected country. Country and Foundation forms use their own destination. “Not sure yet” uses the default selected in Email configuration (initially Uganda). The sending mailbox is the SMTP username and From address; Reply-To is the visitor's email. Recipient addresses are shared with the existing Site settings enquiry-routing fields.

Forms save enquiries before email delivery. A background worker checks the durable queue every ten seconds; SMTP failures do not discard enquiries or fail the visitor's submission. Delivery status and a retry button appear in **Inquiries**. Failed or unconfigured deliveries require a manual retry after correcting settings. Existing enquiries remain unqueued until explicitly sent. If delivery was interrupted, check the inbox before retrying because Hostinger may already have accepted the message. “Sent” means SMTP acceptance, not guaranteed inbox delivery.

The `AddHostingerEmail` migration adds mailbox profiles, the default destination and enquiry delivery fields. The app applies pending migrations on startup; restart the updated application to install the schema before opening Email configuration.

Run the offline routing, encryption and migration checks with `dotnet run --project tests/EmailChecks`. These checks do not connect to a database or send email. Verify actual SMTP delivery using each profile's test button after entering real credentials.

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

## Content refresh (version 2)

Version 2 of the built-in content comes from the fork's content pull request: five new service lines with "What we deliver" lists, a much larger project list with descriptions and new filters (countries: Uganda, Zambia, Burundi, DRC; categories: Construction Services, Technical Studies, Engineering Design, Environmental & Social Impact, Climate Resilience), the revised Who We Are copy and six values, rewritten Uganda and Zambia pages, and a PDF approach document for each SDG goal.

- A new database is seeded straight to version 2.
- An existing database is upgraded the first time the new build starts. Each section (services, capabilities, values, projects, country pages) is replaced only if it still matches the original seeded content; a section that has been edited in the dashboard is left alone and named in a startup warning, so nothing anyone wrote is overwritten. The old projects are kept but hidden from the Projects page.
- Country-page cards can now be links: add a custom service card and give it a link such as `/contact` (that is how "Not sure which service you need?" works).
- Each SDG goal has an optional approach document. Its path is entered in the dashboard; the PDF files themselves live in `wwwroot/docs/sdg` and are added through the repository, not the Media library.

## Backups

Back up the PostgreSQL database and the uploads folder together. The database holds the content, the users and the enquiries.

## Not managed in the dashboard

Fixed copy that lives in the `.cshtml` views is still edited in code: the Foundation page's introductory text, the Who We Are statement and stats, the Services and Projects headings, the Contact page, and the Home page's section headings. `ServicePanelContent` (the Home "what we do" panels) exists as a record but is not used by any page, so it has no screen.
