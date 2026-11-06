# ZooAR — Taller N.º 2, Programación para Realidad Aumentada

Aplicación educativa de realidad aumentada que permite explorar animales
representativos de las tres regiones naturales del Perú.

Universidad ESAN · 2026-2 · Unity 6000.6.1f1 + Vuforia

## Estructura

```
Assets/
├── Editor/
│   ├── GeneradorInterfacesZooAR.cs   Crea las 5 interfaces de un clic
│   └── AsignarRecursosZooAR.cs       Asigna sprites, textos y audio
├── Scripts/
│   ├── AudioManagerZooAR.cs          Sonido de fondo y de las experiencias RA
│   ├── NavegacionZooAR.cs            Navegación entre interfaces
│   ├── MarcadorZooAR.cs              Handler de cada Image/Multi Target
│   └── MarcadorVideoZooAR.cs         Handler del marcador del video
├── ZooAR/
│   ├── Imagenes/                     Fondo, isologo, máscaras, figuras
│   ├── Audio/                        Fondo + un sonido por modelo 3D
│   ├── Video/                        Video educativo
│   └── Modelos/                      Modelos 3D con texturas
└── StreamingAssets/Vuforia/          Bases de datos de marcadores
```

## Las cinco interfaces

| # | Interfaz | Contenido |
|---|---|---|
| 1 | Presentación | Fondo e isologo; al tocarlo arranca el sonido de fondo |
| 2 | Menú principal | Costa, Sierra, Selva y Video Animal |
| 3 | Región Costa | Tres figuras, nombre a la derecha, botón de retorno |
| 4 | Región Sierra | Tres figuras, nombre a la derecha, botón de retorno |
| 5 | Región Selva | Una figura, nombre a la derecha, botón de retorno |

## Animales y marcadores

| Región | Animal | Marcador |
|---|---|---|
| Costa | Pingüino de Humboldt | Retrato |
| Costa | Ballena jorobada | Retrato |
| Costa | Delfín | Retrato |
| Sierra | Alpaca | Set personalizado 01 |
| Sierra | Cóndor andino | Set personalizado 02 |
| Sierra | Chinchilla | Set personalizado 03 |
| Selva | Jaguar | Merge Cube (Cuboid Multi Target) |
| — | Video educativo | Dibujo a mano alzada |

## Lógica del sonido

Es el requisito con más detalle del taller y vive en `AudioManagerZooAR`:

1. El sonido de fondo arranca en la Interfaz 1 y suena durante las cinco.
2. Se detiene **solo** al activar una experiencia de realidad aumentada.
3. Durante la RA suena únicamente el audio del modelo 3D o del video.
4. El botón de regreso detiene el audio de RA y reanuda el de fondo.

`NavegacionZooAR` recuerda desde qué interfaz se entró a la RA, de modo que el
botón de regreso —que es uno solo, compartido— devuelve siempre al sitio
correcto.

## Montaje desde cero

1. Abrir el proyecto en Unity 6000.6.1f1.
2. Menú **ZooAR → Generar interfaces**.
3. Menú **ZooAR → Asignar recursos**.
4. Importar la base de datos de Vuforia y colocar la ARCamera y los targets
   dentro de `RA_Root`.
5. Poner `MarcadorZooAR` en cada target de animal y `MarcadorVideoZooAR` en el
   del dibujo a mano.

## Pendientes

- Modelo 3D del delfín
- Video con pista de audio (el actual es mudo)
- Marcador del dibujo a mano alzada
- Retrato del pingüino en la base de datos de Vuforia

## Créditos de los modelos

Los modelos 3D provienen de repositorios de terceros y conservan sus licencias
originales. El modelo de la alpaca es de uso no comercial.
