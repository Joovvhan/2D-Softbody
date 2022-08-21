import yaml
import matplotlib.pyplot as plt
import numpy as np


def GetOutlineFromYaml(file_name):
    with open(file_name) as f:
        data = yaml.load(f, Loader=yaml.FullLoader)
        
    vertices = VertDictList2Array(data['vertices'])
    edges = EdgeDictList2Indices(data['edges'])
        
    return vertices[edges]


def VertDictList2Array(vert_dict_list):
    
    data = list()
    
    for vert_dict in vert_dict_list:
        data.append([vert_dict['x'], vert_dict['y']])
        
    return np.array(data)
    

def EdgeDictList2Indices(edge_dict_list):
    
    indices = set()
    loop = dict()
    
    for i, edge_dict in enumerate(edge_dict_list):
        x, y = sorted([edge_dict['x'], edge_dict['y']])
        if i == len(edge_dict_list) - 1:
            x, y = y, x
        
        indices.add(x)
        indices.add(y)
        
        if i == 0:
            start = x
        else:
            if x not in loop:
                print("Multiple Loop Detected")
                print(loop)
            if y in loop:
                print("Loop Detected")
                print(loop)
            if i == len(edge_dict_list) - 1:
                if y != start:
                    print(f"End Point Doesn't Match {y} != {start}")
                
        loop[y] = x
        
    return np.array(sorted(list(indices)))


def GetCenterPointFromVectices(vertices):
    x_low, x_high = min(vertices[:, 0]), max(vertices[:, 0])
    y_low, y_high = min(vertices[:, 1]), max(vertices[:, 1])
    
    x_center = int(np.round(np.mean([x_low, x_high])))
    y_center = int(np.round(np.mean([y_low, y_high])))

    return np.array([x_center, y_center])


def GetPreviousAndNextIndex(i, last_index):
    
    previous_index = i-1 if i-1 >= 0 else last_index
    next_index = i+1 if i+1 <= last_index else 0
            
    return previous_index, next_index


def OffsetVertices(vertices, d, mode='normal'):
    
    new_vertices = list()
    
    center_point = GetCenterPointFromVectices(vertices)
    
    last_index = len(vertices) - 1
    
    last_n = None
    
    for i, vertex in enumerate(vertices):
        
        r_vector = center_point - vertex
        r_vector_norm = r_vector / np.sqrt(np.sum(r_vector ** 2))
        
        if mode == 'center':
            offset_vector = d * r_vector_norm 
            
        elif mode == 'normal':
            
            previous_index, next_index = GetPreviousAndNextIndex(i, last_index)
  
            p1, p2 = vertices[previous_index], vertices[next_index]
            p = p2 - p1
            
            n1 = np.array([p[1], -1 * p[0]])
            n2 = np.array([-1 * p[1], p[0]])
            
            # Based on Continous Normal Vector Assumption
            # 
            if last_n is None:
                n = n1 if np.sum(n1 * r_vector_norm) > 0 else n2
            else:
                n = n1 if np.sum(n1 * last_n) > 0 else n2
                
            n = n / np.sqrt(np.sum(n ** 2))
            
            offset_vector = d * n
            last_n = n
            
        else:
            print(f"Invalid Offset Mode [{mode}]")
        
        new_vertex = vertex + offset_vector
        
        new_vertices.append(new_vertex)
        
    new_vertices = np.array(new_vertices)
        
    return new_vertices

def IncreaseVertices(vertices, r=4):
    
    last_index = len(vertices) - 1
    
    new_vectices = np.empty((0, 2))
    
    for i, vertex in enumerate(vertices):
        
        _, next_index = GetPreviousAndNextIndex(i, last_index)
        next_vertex = vertices[next_index]
        xs = np.linspace(vertex[0], next_vertex[0], r, endpoint=False)
        ys = np.linspace(vertex[1], next_vertex[1], r, endpoint=False)
        
        new_points = np.stack([xs, ys], axis=1)
        
        new_vectices = np.concatenate((new_vectices, new_points), axis=0)
    
    return new_vectices


def GetlineLengthFromVertices(vertices):
    
    last_index = len(vertices) - 1
    
    total_distance = 0

    for i, vertex in enumerate(vertices):

        _, next_index = GetPreviousAndNextIndex(i, last_index)
        
        d = GetVertexDistance(vertex, vertices[next_index])

        total_distance += d

    return total_distance


def GetVertexDistance(v0, v1):
    
    x0, y0 = v0
    x1, y1 = v1
    d = np.sqrt((x1-x0)**2 + (y1-y0)**2)
    
    return d
    

def SampleVertices(vertices, target_vertex_num=None, target_distance=None):
    
    assert not (target_vertex_num == None and target_distance == None), "Target Vertex Number and Target Distance Should Not be both None"
    
    last_index = len(vertices) - 1
    
    total_distance = GetlineLengthFromVertices(vertices)
    
    if target_vertex_num is not None:
        target_distances = np.linspace(0, total_distance, target_vertex_num, endpoint=False)
    elif target_distance is not None:
        target_vertex_num = int(np.ceil(total_distance / target_distance))
        target_distances = np.linspace(0, total_distance, target_vertex_num, endpoint=False)
    
    cummulated_distance = list()
    distance = 0
    for i, vertex in enumerate(vertices):
        _, next_index = GetPreviousAndNextIndex(i, last_index)
        d = GetVertexDistance(vertex, vertices[next_index])
        distance += d
        cummulated_distance.append(distance)
        
    cummulated_distance = np.array(cummulated_distance)
        
    selected_vertices = [0]
    for target in target_distances[1:]:
        i = np.argmin(np.abs(cummulated_distance - target))
        selected_vertices.append(i+1)

    selected_vertices = np.array(selected_vertices)

    return vertices[selected_vertices]


def DrawOffsetVectors(vertices, new_vertices):
    for vertex, new_vertex in zip(vertices, new_vertices):
        direction = new_vertex - vertex
        plt.arrow(vertex[0], vertex[1], direction[0], direction[1], head_width=4, fc='black')
    
    
def DrawOffsetCircles(vertices, r):

    ax = plt.gca()
    
    for x, y in vertices:
        circle = plt.Circle((x, y), r, color='b', fill=False)
        ax.add_patch(circle)
