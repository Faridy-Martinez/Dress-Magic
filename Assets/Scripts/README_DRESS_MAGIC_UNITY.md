# Dress & Magic - primera base jugable en Unity

## Qué incluye
- Menú de inicio.
- Selección de los seis escenarios definitivos.
- Vestidor por categorías: ropa, zapatos, peinados y accesorios.
- Visualización de prendas como capas superpuestas sobre la muñeca.
- Pantalla de colección y guardado de desbloqueos con PlayerPrefs.
- Pantalla final del outfit.
- Estructura ampliable para incorporar prendas futuras.

## Instalación
1. Abre el proyecto Unity.
2. En Assets crea las carpetas `Scripts` y `Resources/DressMagic` si todavía no existen.
3. Copia los scripts `DressMagicGame.cs` y `DressMagicSceneBootstrap.cs` dentro de `Assets/Scripts`.
4. En la escena inicial, crea un GameObject vacío llamado `DressMagicManager`.
5. Añade el componente `DressMagicSceneBootstrap` a ese objeto.
6. Prepara los recursos gráficos siguiendo `Assets/Resources/DressMagic/README_RECURSOS.txt`.
7. Espera a que Unity termine de importar los sprites y pulsa Play.

## Importante: ilustración original
El juego no dibuja una muñeca sustituta. Carga el archivo `Doll.png) que se exporte desde la ilustración original. Para conservar su aspecto, usa el mismo lienzo y proporciones para todas las capas. Las prendas del README todavía deben exportarse por separado como PNG transparentes; una imagen de catálogo no se puede convertir de forma fiable en capas sin separar cada pieza.

## Combinaciones mágicas
En el Inspector del componente `DressMagicGame`, aumenta el arreglo `Combinations` y asigna:
- Id: identificador único del desbloqueo.
- First Item / Second Item: nombres exactos de dos sprites que forman la combinación.
- Reward Item: nombre del recurso que se desbloquea.
- Message: mensaje que aparece al descubrirla.

El desbloqueo se conserva aunque se cierre el juego.

## Agregar prendas nuevas
Solo copia el PNG de la prenda a la carpeta de su categoría. No es necesario modificar el código. Usa un nombre descriptivo y mantén el lienzo alineado con la muñeca.

## Nota de alcance
Esta es la base de programación e interfaz. La imagen de la muñeca y las prendas finales deben exportarse y probarse dentro de Unity antes de considerar terminado el vestidor visual.
