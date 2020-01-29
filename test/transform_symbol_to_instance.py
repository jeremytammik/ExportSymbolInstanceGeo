from pyrr import *

symbol_rotation = [[-0.9816,0.1908,0], [-0.1908,-0.9816,0], [0,0,1]]
symbol_translation = [-1935,3285,0]
m4=Matrix44(symbol_rotation)
translation_matrix = matrix44.create_from_translation(symbol_translation)
matrix = matrix44.multiply(m4, translation_matrix)
p=[-140,223,978]
print(matrix44.apply_to_vector(matrix, p))
