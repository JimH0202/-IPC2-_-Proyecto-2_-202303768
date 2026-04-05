from pathlib import Path
path = Path('test-input-200-drones.xml')
with path.open('w', encoding='utf-8') as f:
    f.write('<?xml version="1.0" encoding="UTF-8"?>\n')
    f.write('<configuracion>\n')
    f.write('  <listaDrones>\n')
    for i in range(1, 201):
        f.write(f'    <dron>DRON{i:03d}</dron>\n')
    f.write('  </listaDrones>\n')
    f.write('  <listaSistemasDrones>\n')
    f.write('    <sistemaDrones nombre="Demo">\n')
    f.write('      <alturaMaxima>100</alturaMaxima>\n')
    f.write('      <cantidadDrones>200</cantidadDrones>\n')
    for i in range(1, 201):
        height = ((i - 1) % 100) + 1
        f.write('      <contenido>\n')
        f.write(f'        <dron>DRON{i:03d}</dron>\n')
        f.write('        <alturas>\n')
        f.write(f'          <altura valor="{height}">L{height}</altura>\n')
        f.write('        </alturas>\n')
        f.write('      </contenido>\n')
    f.write('    </sistemaDrones>\n')
    f.write('  </listaSistemasDrones>\n')
    f.write('  <listaMensajes>\n')
    f.write('    <Mensaje nombre="MENSAJE1">\n')
    f.write('      <sistemaDrones>Demo</sistemaDrones>\n')
    f.write('      <instrucciones>\n')
    for i in range(1, 11):
        height = ((i - 1) % 100) + 1
        f.write(f'        <instruccion dron="DRON{i:03d}">{height}</instruccion>\n')
    f.write('      </instrucciones>\n')
    f.write('    </Mensaje>\n')
    f.write('  </listaMensajes>\n')
    f.write('</configuracion>\n')
