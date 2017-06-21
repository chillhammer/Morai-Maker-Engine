import csv, sys

# TODO Pass in level as args instead of reading a file
with open(sys.argv[1], 'rb') as level_file:
    reader = csv.reader(level_file)

    # Get grid size
    grid_size = reader.next()
    grid_size = (int(grid_size[0]), int(grid_size[1]))

    # Get grid objects
    for row in reader:
        print ', '.join(row)

    # Create additions
    additions = []
    additions.append(['Block 1', 5, 7])
    additions.append(['Block 1', 25, 7])
    additions.append(['Block 1', 45, 7])
    additions.append(['Block 1', 85, 7])

    # Write additions to file
    with open('additions.csv', 'wb') as additions_file:
        writer = csv.writer(additions_file)
        for addition in additions:
            writer.writerow(addition)