const { spawn, spawnSync } = require('node:child_process')
const path = require('node:path')

const spaDirectory = path.resolve(__dirname, '..')
const repositoryDirectory = path.resolve(spaDirectory, '..')
const apiAssembly = path.join(
  repositoryDirectory,
  'telemedicine-webapi',
  'src',
  'telemedicine.api',
  'bin',
  'Release',
  'net10.0',
  'telemedicine.api.dll',
)
const viteCli = path.join(path.dirname(require.resolve('vite/package.json')), 'bin', 'vite.js')
const playwrightCli = require.resolve('@playwright/test/cli')
const children = []

function start(command, args, options = {}) {
  const child = spawn(command, args, {
    cwd: options.cwd || spaDirectory,
    env: { ...process.env, ...options.env },
    stdio: ['ignore', 'pipe', 'pipe'],
    windowsHide: true,
  })
  child.stdout.on('data', (data) => process.stdout.write(data))
  child.stderr.on('data', (data) => process.stderr.write(data))
  children.push(child)
  return child
}

async function waitFor(url, processToWatch, timeoutMs = 180000) {
  const deadline = Date.now() + timeoutMs
  while (Date.now() < deadline) {
    if (processToWatch.exitCode !== null) {
      throw new Error(`${url} process exited with code ${processToWatch.exitCode}`)
    }
    try {
      const response = await fetch(url)
      if (response.ok) return
    } catch (_) {
      // Startup polling is expected to fail until the listener is ready.
    }
    await new Promise((resolve) => setTimeout(resolve, 500))
  }
  throw new Error(`Timed out waiting for ${url}`)
}

function stop(child) {
  if (!child || child.exitCode !== null) return
  if (process.platform === 'win32') {
    spawnSync('taskkill.exe', ['/pid', String(child.pid), '/t', '/f'], {
      stdio: 'ignore',
      windowsHide: true,
    })
  } else {
    child.kill('SIGTERM')
  }
}

async function main() {
  let exitCode = 1
  try {
    const api = start('dotnet', [apiAssembly, '--urls', 'http://127.0.0.1:5231'], {
      cwd: path.dirname(apiAssembly),
      env: {
        ASPNETCORE_ENVIRONMENT: 'Development',
        DefaultConnection: 'Host=localhost;Database=telemedicine_test;Username=postgres;Password=worisoft@123',
        ConnectionStrings__DefaultConnection: 'Host=localhost;Database=telemedicine_test;Username=postgres;Password=worisoft@123',
        JwtSettings__Key: 'bkoRhqz4meFCPauYpjyIlZZDCO59rRjrH4IRloPdvg8=',
        DemoData__Enabled: 'true',
        DemoData__ResetConsultations: 'true',
        Database__ApplyMigrationsOnStartup: 'true',
        RateLimiting__Authentication__PermitLimit: '1000',
        RateLimiting__Payment__PermitLimit: '1000',
        RateLimiting__Scheduling__PermitLimit: '1000',
      },
    })
    await waitFor('http://127.0.0.1:5231/health/live', api)

    const vite = start(process.execPath, [viteCli, '--host', '127.0.0.1', '--port', '8080', '--strictPort'], {
      env: { VITE_API_TARGET: 'http://127.0.0.1:5231' },
    })
    await waitFor('http://127.0.0.1:8080/', vite)

    const tests = start(process.execPath, [playwrightCli, 'test', ...process.argv.slice(2)])
    exitCode = await new Promise((resolve) => tests.once('exit', (code) => resolve(code ?? 1)))
  } catch (error) {
    console.error(error)
  } finally {
    for (const child of children.reverse()) stop(child)
  }
  process.exit(exitCode)
}

main()
