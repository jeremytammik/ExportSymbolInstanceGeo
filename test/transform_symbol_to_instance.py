import json
from pyrr import *

# get instance transformation data

with open("instance_transform.json") as f:
  d = json.loads(f.read())

symbol_rotation = d['symbol_rotation']
symbol_translation = d['symbol_translation']

# create transformation matrix

mrot = Matrix44(symbol_rotation)
mtrans = matrix44.create_from_translation(symbol_translation)
matrix = matrix44.multiply(mrot, mtrans)

# compare transformed symbol vertices with equivalent instance vertices

with open("symbol_geometry.json") as f:
  dsymbol = json.loads(f.read())

with open("instance_geometry.json") as f:
  dinstance = json.loads(f.read())
  
symbol_solids = dsymbol['solids']
instance_solids = dinstance['solids']

nsolids = len(symbol_solids)

print('%d symbol and %d instance solids' % (nsolids, len(instance_solids)))

n_points_tested = 0

for i in range(nsolids):
  solsym = symbol_solids[i]
  solinst = instance_solids[i]
  nfaces = len(solsym)
  print('%d symbol and %d instance faces' % (nfaces, len(solinst)))
  for j in range(nfaces):
    fsym = solsym[j]
    finst = solinst[j]
    ntriangles = len(fsym)
    print('%d symbol and %d instance triangles' % (ntriangles, len(finst)))
    for k in range(ntriangles):
      tsym = fsym[k]
      tinst = finst[k]
      nvertices = len(tsym)
      print('%d symbol and %d instance triangle vertices' % (nvertices, len(tinst)))
      for l in range(nvertices):
        psym = tsym[l]
        psymx = matrix44.apply_to_vector(matrix, psym)
        pinst = Vector3(tinst[l])
        vdiff = pinst - psymx
        dist = vector.length(vdiff)
        print ('symbol vertex', psym, 'xform:', psymx, '~=', tinst[l], 'diff', '{0:.2f}'.format(dist), 'mm')
        assert 1 > dist, 'expected distances less than 1 mm'
        n_points_tested += 1

print( n_points_tested, 'points tested, all transformed symbol vertices are within 1 mm of the corresponding instance vertices')
