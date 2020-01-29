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

print('%d symbol and %d instance solids' % (len(symbol_solids), len(instance_solids)))

#p=[-140,223,978]
#print(matrix44.apply_to_vector(matrix, p))
