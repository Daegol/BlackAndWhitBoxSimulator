   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      J FDiagramDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner 5      C&clr-namespace:DiagramDesigner.ControlsDiagramDesigner.Controls 5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   DesignerItemContextMenu.   (System.Windows.Input.ApplicationCommands
     Cut m� Command m� Icon    Copy    Paste    Delete  DiagramDesigner.Connector �� 
TargetType �� Property �� Value	 �� 
TargetType  ConnectorDecoratorTemplate
  Name   Orientation  ResizeDecoratorTemplate J� Opacity J� SnapsToDevicePixels*  $DiagramDesigner.Controls.ResizeThumb � Cursor(  "DiagramDesigner.Controls.DragThumb"  DiagramDesigner.DesignerItem � ContextMenu � DataContext �� RelativeSource  Name ��Name ��Name �� Triggers �� Value �� Binding �� Path �� 
TargetName ^� 
Conditions �� Binding �� Value �� Empty P� Property P� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .sys&clr-namespace:System;assembly=mscorlib  5      #sclr-namespace:DiagramDesigner  5      ,c&clr-namespace:DiagramDesigner.Controls  5      %	  &	        '      &	 �    &	 5    '  	    '  �	    7   7  7  �� 5      .@�5   
   m� $	��Cut��6   # Z  6     5	      �� 5
      $��Images/Cut.png��6   $��16��60   5      m� 5   
   $
��Copy��6   # Z 6!    5      �� 5      $��Images/Copy.png��6   $��16��61   5      m� 5   
   $��Paste��6   # Z 6"    5      �� 5      $��Images/Paste.png��6   $��16��62   5      m� 5   
   $��Delete��6   # Z 6#    5      �� 5      $��Images/Delete.png��6   $��16��63   5      �� 5   
   5      �� 5       "  6   .�5!   
   ��  � ��$ 8��6"   �� 5"   
    � ��$ 8��6#   �� 5#   
    �@�Cursor$ Cross��6#   �� 5$   
    �@J�SnapsToDevicePixels . 60   �� 5%   
    � �� 5&      �� 5'      "	  6"   .R�5(      � .+�5*      �� 
������ $��-2q�67   �� 5+      
�������$��1��64   ���	#AA0000806H   5,      5-      5/      50      �� 53      "	 ��69   .R�54   
   � $��-5q�6   .+�55        $

 Left��6   $
 Left=�6(   $��Center=�6;   $
��Left=�6V     56      $	
 Top��6   $	 Top=�6'   $	��Top=�69   $��Center=�6Q     57      $
 Right��6   $ Right=�6)   $��Center=�6=   $��Right=�6X     58      $
 Bottom��6   $ Bottom=�6*   $��Bottom=�6?   $��Center=�6Z   59      5:      �� 5=      "	 ��66   .R�5>   
   � $	 0.7Q�6    . .+�5?        $��3��6   $ SizeNS��6'   $��0 -4 0 0q�67   $	��Top=�5@      $��Stretch=�6.     5A      $��3��6   $ SizeWE��6&   $��-4 0 0 0q�66   $��Stretch=�5B      $
��Left=�62     5C      $��3��6   $ SizeWE��6&   $��0 0 -4 0q�66   $��Stretch=�5D      $��Right=�62     5E      $��3��6   $ SizeNS��6'   $��0 0 0 -4q�67   $��Bottom=�5F      $��Stretch=�61     5G      $��7��6   $��7��6&   $ SizeNWSE��61   $��	-6 -6 0 0q�6C   $	��Top=�5H      $
��Left=�6.     5I      $��7��6   $��7��6&   $ SizeNESW��61   $��	0 -6 -6 0q�6C   $	��Top=�5J      $��Right=�6.     5K      $��7��6   $��7��6&   $ SizeNESW��61   $��	-6 0 0 -6q�6C   $��Bottom=�5L      $
��Left=�61     5M      $��7��6   $��7��6&   $ SizeNWSE��61   $��	0 0 -6 -6q�6C   $��Bottom=�5N      $��Right=�61   5O      5P      �� 5S      "  6   .�5T   
   ��  � �� 5U      �� 5V      "	  6"   .R�5W      �� 
������ 6    5X      5Z      5[      �� 5^      "  6   .�5_   
   ��  � ��$ 10��6%   �� 5`   
    � ��$ 10��6&   �� 5a   
    �@J�SnapsToDevicePixels . 60   �� 5b   
    � �� 5c      �� 5d      "	  6"   .R�5e      � 8    6   ��  �� *TemplatedParent+.+�5h        $ PART_DragThumb��6&   $ SizeAll��5i   &   �� 5k      $ PART_ResizeDecorator��6"   $y�	Collapsed=�5l   "   8�� 5m   "   �� 5o      $ PART_ContentPresenter��6+   $��Stretch=�5p   +   $��Stretch=�5q   +   #��z��5r   +   #��z��5s   +   �� 5u      $ PART_ConnectorDecorator��6"   $y�Hidden=�5v   "   8�� 5w   "   5x       5y      �� 5z      $
 True��6&    63   ��  �� *Self+$ IsGroup�.a�5{      �� $ PART_DragThumb��6%    � y�$ 	Collapsed=�6W   5|      ^� 5}       5~      �� 5   "    69   ��  �� *Self+$ 
IsSelected� True6,   69   �� 5�   "   # Z 6,    6N   ��  �� *Self+$ ParentID�.0�5�      �� $ PART_ResizeDecorator��6%    � y�$ Visible=�6]   5�      P� 5�       �@J�IsMouseOver . . �5�      �� $ PART_ConnectorDecorator��6%    � y�$ Visible=�6`   5�      �� 5�      $
 True��6&    63   ��  �� *Self+$ IsDragConnectionOver�.a�5�      �� $ PART_ConnectorDecorator��6%    � y�$ Visible=�6`   5�      �� 5�      $
 True��6&    63   ��  �� *Self+$ IsGroup�.a�5�      �� $ PART_ConnectorDecorator��6%    � y�$ Hidden=�6`   5�      5�      5�      5�      5�      