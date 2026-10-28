// Tiny local static server for HTTP and HTTPS origins accepted by the API Gateway CORS policy.
const http = require('node:http');
const https = require('node:https');
const fs = require('node:fs');
const path = require('node:path');

const root = __dirname;
const types = { '.html': 'text/html; charset=utf-8', '.css': 'text/css; charset=utf-8', '.js': 'text/javascript; charset=utf-8', '.json': 'application/json; charset=utf-8' };
const publicFiles = new Set(['trangchu.html', 'styles.css', 'app.js', 'enhancements.js', 'config.js']);

function serve(req, res) {
  const pathname = decodeURIComponent(new URL(req.url, 'http://localhost').pathname);
  const relative = pathname === '/' ? 'trangchu.html' : pathname.replace(/^\/+/, '');
  if (!publicFiles.has(relative) || req.method !== 'GET') {
    res.writeHead(404).end('Not found');
    return;
  }
  const file = path.resolve(root, relative);
  if (!file.startsWith(root + path.sep)) {
    res.writeHead(403).end('Forbidden');
    return;
  }
  fs.readFile(file, (error, content) => {
    if (error) {
      res.writeHead(error.code === 'ENOENT' ? 404 : 500).end('Not found');
      return;
    }
    res.writeHead(200, { 'Content-Type': types[path.extname(file)] || 'application/octet-stream', 'Cache-Control': 'no-store' });
    res.end(content);
  });
}

const httpPort = Number(process.env.WORKLY_HTTP_PORT || 5500);
const httpsPort = Number(process.env.WORKLY_HTTPS_PORT || 5501);
const pfxPath = process.env.WORKLY_HTTPS_PFX;
const pfxPassword = process.env.WORKLY_HTTPS_PFX_PASSWORD;

http.createServer(serve).listen(httpPort, () => console.log(`Workly frontend: http://localhost:${httpPort}`));

if (pfxPath) {
  try {
    const credentials = { pfx: fs.readFileSync(pfxPath), passphrase: pfxPassword };
    https.createServer(credentials, serve).listen(httpsPort, () => console.log(`Workly frontend: https://localhost:${httpsPort}`));
  } catch (error) {
    console.error(`Could not load the HTTPS certificate: ${error.message}`);
    process.exitCode = 1;
  }
} else {
  console.warn('HTTPS is disabled. Set WORKLY_HTTPS_PFX to enable it.');
}
