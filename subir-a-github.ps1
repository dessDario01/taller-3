# Sube el proyecto ZooAR a GitHub.
#
# Uso: abre PowerShell en la carpeta del proyecto y ejecuta
#
#     .\subir-a-github.ps1
#
# Si Windows bloquea la ejecucion de scripts, corre antes:
#
#     Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
#
# La primera vez GitHub te va a pedir que inicies sesion: se abre el
# navegador o te pide un token. Eso lo haces tu; el script no maneja
# credenciales.

$ErrorActionPreference = "Stop"

$repo = "https://github.com/dessDario01/taller-3.git"
$proyecto = $PSScriptRoot

Write-Host ""
Write-Host "=== Subiendo ZooAR a GitHub ===" -ForegroundColor Cyan
Write-Host "Proyecto: $proyecto"
Write-Host "Repo:     $repo"
Write-Host ""

Set-Location $proyecto

# --- Comprobaciones ---

try {
    $v = git --version
    Write-Host "[ok] $v"
} catch {
    Write-Host "[X] Git no esta instalado o no esta en el PATH." -ForegroundColor Red
    Write-Host "    Descargalo de https://git-scm.com/download/win y vuelve a intentar."
    exit 1
}

if (-not (Test-Path ".gitignore")) {
    Write-Host "[!] No encuentro .gitignore. Sin el subirias la carpeta Library," -ForegroundColor Yellow
    Write-Host "    que pesa cientos de MB. Aborto por seguridad." -ForegroundColor Yellow
    exit 1
}

# --- Repositorio local ---

if (-not (Test-Path ".git")) {
    Write-Host "[..] Inicializando repositorio"
    git init | Out-Null
} else {
    Write-Host "[ok] El repositorio ya estaba inicializado"
}

git branch -M main

# --- Remoto ---

$remotos = git remote
if ($remotos -contains "origin") {
    Write-Host "[..] Actualizando el remoto origin"
    git remote set-url origin $repo
} else {
    Write-Host "[..] Agregando el remoto origin"
    git remote add origin $repo
}

# --- Commit ---

git add -A

$cambios = git status --porcelain
if ([string]::IsNullOrWhiteSpace($cambios)) {
    Write-Host "[ok] No hay cambios nuevos que confirmar"
} else {
    Write-Host "[..] Creando commit"
    $mensaje = "ZooAR: interfaces, scripts, recursos y marcadores"
    git commit -m $mensaje -m "Co-Authored-By: Claude Opus 5 <noreply@anthropic.com>"
}

# --- Push ---

Write-Host ""
Write-Host "[..] Subiendo a GitHub. Puede pedirte iniciar sesion." -ForegroundColor Cyan
Write-Host ""

git push -u origin main
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "[!] El push fue rechazado. Suele pasar cuando el repo ya tenia" -ForegroundColor Yellow
    Write-Host "    commits (un README creado desde la web). Reconcilio y reintento." -ForegroundColor Yellow
    Write-Host ""

    git pull origin main --rebase --allow-unrelated-histories
    git push -u origin main
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "=== Listo ===" -ForegroundColor Green
    Write-Host "https://github.com/dessDario01/taller-3"
} else {
    Write-Host ""
    Write-Host "[X] El push siguio fallando. Copia el error y pasamelo." -ForegroundColor Red
}
