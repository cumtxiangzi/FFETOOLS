   M S B A M L   `   `   `  ����G  CDrawingTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.CreatDrawings   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5          � Title$  批量创建图纸��5   	   $	��300��6   $	��320��6%    � 
ResizeMode$ NoResize=�61    � WindowStartupLocation$ CenterScreen=�6^   .��5	      � .+�5
   
   �� -   5         ��Name$ DrawingTypeCombo��5
      $
��Left=�6-   $��23��6H   $��
138,27,0,0q�6T   $	��Top=�6h   $	��135��6�   6�   �� 5   
   $
��Left=�6   $��23��6/   $��	14,28,0,0q�6;   $
��Wrap=�6N   $��请选择图纸类型��6b   $	��Top=�6q   $	��109��6�    �� TextAlignment$ Center=�6�   $��14(�6�   �� 5   
   $
��Left=�6   $��21��6/   $��
17,129,0,0q�6;   $
��Wrap=�6O   $	��Top=�6c   $	��102��6{   $��请输入图纸数量��6�   $��14(�6�   $ Center=�6�   �� 5   
   -   5         ��Name$ DrawingNumber��5      $
��Left=�6)   $��21��6D   $��138,129,0,0q�6P    �� TextWrapping$
 Wrap=�6e   $	��Top=�6y   $	��135��6�   �� 5   
   -   5        $��确定��5      $
��Left=�6   $��24��69   $��226,220,0,0q�6E   $	��Top=�6Z   $��64��6r    � Cursor$
 Hand��6�   �� 5   
   $
��Left=�6   $��21��6/   $��	17,80,0,0q�6;   $
��Wrap=�6N   $	��Top=�6b   $	��102��6z   $��请选择使用专业��6�   $��14(�6�   $ Center=�6�   �� 5   
   -   5        $ 
MajorCombo��5      $
��Left=�6'   $��23��6B   $��
138,78,0,0q�6N   $	��Top=�6b   $	��135��6z   6�   �� 5   
   $
��Left=�6   $��21��6/   $��
17,176,0,0q�6;   $
��Wrap=�6O   $	��Top=�6c   $	��102��6{   $��14(�6�   $ Center=�6�   �� 6�   $	�请��6�   �� 6�   $�选择语言版本��6�   6�   	� 5   
   -   5         	�Name$ 	CH_Button��5      $��中文��6)   $
��Left=�66   $��138,179,0,0q�6Q   $	��Top=�6f   	� 5   
   -   5        $ 	EN_Button��5      $��英文��6)   $
��Left=�66   $��187,179,0,0q�6Q   $	��Top=�6f   	� 5   
   -   5        $ CH_EN_Button��5      $��	中英文��6,   $
��Left=�6:   $��235,179,0,0q�6U   $	��Top=�6j   �� 5   
   -	   5        	 ��Name$	 
CreatTitle��5      $
��Left=�6(   $��
20,223,0,0q�6C   $	��Top=�6W   $��创建标题栏��6o   $��14(�6   5      5      