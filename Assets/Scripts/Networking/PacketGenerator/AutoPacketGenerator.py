import os
import shutil
import sys
import time

# Path to packets
PATH = "..\\Packets"

start_time = time.time()

with open("Packets.txt", "r") as f:
    all_packets_split = f.read().split("\n")

index = 0
while index < len(all_packets_split):
    # Remove lines that don't contain '\' and thus are comments
    if "\\" not in all_packets_split[index]:
        all_packets_split.pop(index)
    else:
        index += 1

# Split all commands
all_packets = [i.split("\\") for i in all_packets_split]

# Delete all existing packets
for filename in os.listdir(PATH):
    file_path = os.path.join(PATH, filename)
    try:
        if os.path.isfile(file_path) or os.path.islink(file_path):
            os.unlink(file_path)
        elif os.path.isdir(file_path):
            shutil.rmtree(file_path)
    except Exception as e:
        print('Failed to delete %s. Reason: %s' % (file_path, e))

# Create packet
for j in all_packets:
    # Extract UID and name
    uid = int(j[0])
    packet_name = j[1]

    # Add default attribute
    attributes = [["UID", "int"]]

    # Add all other attributes
    x = 2
    while x < len(j):
        attributes.append([j[x], j[x+1]])
        x += 2

    # Set the filename
    filename = f"{uid}_{packet_name}Packet.cs"

    # Add common start of file
    data = f"""using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Networking.Packets.Generated 
{{
    public class {packet_name}Packet {{
        /// <summary> Unique packet type identifier for {packet_name} </summary>
"""

    # Add UID attribute
    data += f"        public const int UID = {uid};\n"

    # Add all other attributes
    for i in attributes[1:]:
        splitted = i[0].split("=") # Remove default value e.g. a="2" -> a
        data += "        public " + i[1] + " " + splitted[0] + ";" + "\n"

    # Add common lines
    data += f"""        
        /// <summary>
        /// Decodes generic packet data into a {packet_name}Packet
        /// </summary>
        public {packet_name}Packet(Packet packet){{
"""

    # Add attributes to constructor
    for j, i in enumerate(attributes[1:]):
        splitted = i[0].split("=")

        data += "            " + splitted[0] + " = "

        # Add decoder based on type
        if i[1] == "string":
            data += "Encoding.ASCII.GetString"
        elif i[1] == "int":
            data += "BitConverter.ToInt32"
        elif i[1] == "double":
            data += "BitConverter.ToDouble"
        elif i[1] == "byte[]":
            pass
        else:
            print(f"Unsupported type: {i[1]}")
            sys.exit()

        data += f"(packet.Contents[{j}]);\n"

    # Add common line
    data += f"""        }}

        /// <summary>
        /// Creates an encoded {packet_name}Packet from arguments
        /// </summary>
        /// <returns>byte[] containing encoded data</returns>
"""

    # Add common line
    data += """        public static byte[] Build("""

    # Add attributes to build method parameters
    for i in attributes[1:]:
        splitted = i[0].split("=")
        data += i[1] + " _" + splitted[0]
        if len(splitted) > 1:
            data += "=" + splitted[1]
        data += ", "

    if len(attributes) > 1:
        data = data[:-2]

    # Add common line
    data += """) {
            List<byte[]> contents = new List<byte[]>
            {
"""

    # Add encoded attributes to contents
    for i in attributes[1:]:
        splitted = i[0].split("=")
        if i[1] == "string":
            data += f"                Encoding.ASCII.GetBytes(_{splitted[0]}),\n"
        elif i[1] == "byte[]":
            data += f"                _{splitted[0]},\n"
        else:
            data += f"                BitConverter.GetBytes(_{splitted[0]}),\n"

    # Add common line
    data += """            };
            return PacketBuilder.Build(UID, contents);
        }
    }
}"""

    # Write file
    with open(f"{PATH}\\{filename}", "w+") as f:
        f.write(data)

print(f"Time taken: {round((time.time()-start_time)*1000, 2)}ms")