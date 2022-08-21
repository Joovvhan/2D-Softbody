import matplotlib.pyplot as plt

from vertex_utils import *

if __name__ == "__main__":

    file_names = ['./shape_yaml/pudding.yaml', 
                  './shape_yaml/cookie.yaml',
                  './shape_yaml/icecream.yaml', 
                  './shape_yaml/donut.yaml']

    r = 18

    for file_name in file_names:
        
        vertices = GetOutlineFromYaml(file_name)
        
        new_vertices = OffsetVertices(vertices, r)
        
        plt.figure(figsize = (9, 9))
        plt.scatter(vertices[:, 0], vertices[:, 1])
        plt.scatter(new_vertices[:, 0], new_vertices[:, 1])
        DrawOffsetVectors(vertices, new_vertices)
        plt.show()
        
        dense_vertices = IncreaseVertices(new_vertices, 5)
        # sparse_vertices = SampleVertices(dense_vertices, target_vertex_num=48)
        sparse_vertices = SampleVertices(dense_vertices, target_distance=1.5*r)
        
        plt.figure(figsize = (9, 9))
        plt.scatter(vertices[:, 0], vertices[:, 1])
        plt.scatter(dense_vertices[:, 0], dense_vertices[:, 1])
        plt.scatter(sparse_vertices[:, 0], sparse_vertices[:, 1])
        plt.plot(sparse_vertices[0, 0], sparse_vertices[0, 1], '*', color='r', markersize=16)
        plt.plot(sparse_vertices[-1, 0], sparse_vertices[-1, 1], '*', color='b', markersize=16)
        DrawOffsetCircles(sparse_vertices, r)
        plt.show()
        