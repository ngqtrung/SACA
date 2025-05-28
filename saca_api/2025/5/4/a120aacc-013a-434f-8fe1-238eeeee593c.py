import math

class Triangle:
    #Begin your statements here
    
    #End your statements

#DO NOT ADD NEW OR CHANGE STATEMENTS IN BELOW FUNCTION
def display_menu():
    print("1. Test get_perimeter()")
    print("2. Test get_area()")
    print("Enter TC(1/2):")

#=============DO NOT ADD NEW OR CHANGE THE STATEMENTS IN THE MAIN FUNCTION========    
def main():
    print("\nTEST Q4 (2 marks):")

    triangle = Triangle(0, 0, 0)
    edgeA = int(input("Enter edge A: "))
    edgeB = int(input("Enter edge B: "))
    edgeC = int(input("Enter edge C: "))
    triangle = Triangle(edgeA, edgeB, edgeC)

    choice = int(input("Enter TC(1/2): "))
    #=====DO NOT ADD NEW OR CHANGE STATEMENTS AFTER THIS LINE=====
    #==THE OUTPUT AFTER THIS LINE WILL BE USED TO MARK YOUR PROGRAM==
    print("\nOUTPUT:")
    if choice == 1:
        print(f"{triangle.get_perimeter():.2f}")
    elif choice == 2:
        print(f"{triangle.get_area():.2f}")
#End def main

if __name__ == "__main__":
    main()
#====================================================================================