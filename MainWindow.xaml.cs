using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace BilliardApp
{
    public partial class MainWindow : Window
    {
        private Model3DGroup modelGroup;
        private TranslateTransform3D cueBallTransform;
        private TranslateTransform3D cueTransform;

        public MainWindow()
        {
            InitializeComponent();
            Initialize3DModels();
            SimulateCueStrike();
        }

        private void Initialize3DModels()
        {
            modelGroup = new Model3DGroup();

            // Create the cue ball
            var cueBallGeometry = CreateSphere(new Point3D(0, 0, 0), 0.1, 16, 16);
            var cueBallMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.White));
            var cueBallModel = new GeometryModel3D(cueBallGeometry, cueBallMaterial);
            cueBallTransform = new TranslateTransform3D(0, 0.1, 0); // Initial position
            cueBallModel.Transform = cueBallTransform;
            modelGroup.Children.Add(cueBallModel);

            // Create the cue
            var cueGeometry = new MeshGeometry3D
            {
                Positions = new Point3DCollection(new[]
                {
                    new Point3D(-0.05, 0.1, 0),
                    new Point3D(0.05, 0.1, 0),
                    new Point3D(0.05, 0.1, -1),
                    new Point3D(-0.05, 0.1, -1)
                }),
                TriangleIndices = new Int32Collection(new[] { 0, 1, 2, 0, 2, 3 })
            };
            var cueMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Brown));
            var cueModel = new GeometryModel3D(cueGeometry, cueMaterial);
            cueTransform = new TranslateTransform3D(0, 0, 0);
            cueModel.Transform = cueTransform;
            modelGroup.Children.Add(cueModel);

            // Add light to the scene
            var light = new DirectionalLight
            {
                Color = Colors.White,
                Direction = new Vector3D(-1, -1, -1)
            };
            modelGroup.Children.Add(light);

            // Set up the Viewport3D
            var modelVisual = new ModelVisual3D { Content = modelGroup };
            viewport3d.Children.Add(modelVisual);
        }

        private void SimulateCueStrike()
        {
            var cueAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(1)
            };

            var ballAnimation = new DoubleAnimation
            {
                From = 0,
                To = 2.0,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                BeginTime = TimeSpan.FromSeconds(1)
            };

            cueAnimation.Completed += (s, e) =>
            {
                cueBallTransform.BeginAnimation(TranslateTransform3D.OffsetZProperty, ballAnimation);
            };

            cueTransform.BeginAnimation(TranslateTransform3D.OffsetZProperty, cueAnimation);
        }

        private MeshGeometry3D CreateSphere(Point3D center, double radius, int thetaDiv, int phiDiv)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int theta = 0; theta <= thetaDiv; theta++)
            {
                double thetaRad = Math.PI * theta / thetaDiv;
                for (int phi = 0; phi <= phiDiv; phi++)
                {
                    double phiRad = 2 * Math.PI * phi / phiDiv;
                    double x = center.X + radius * Math.Sin(thetaRad) * Math.Cos(phiRad);
                    double y = center.Y + radius * Math.Cos(thetaRad);
                    double z = center.Z + radius * Math.Sin(thetaRad) * Math.Sin(phiRad);
                    mesh.Positions.Add(new Point3D(x, y, z));
                }
            }

            for (int theta = 0; theta < thetaDiv; theta++)
            {
                for (int phi = 0; phi < phiDiv; phi++)
                {
                    int a = theta * (phiDiv + 1) + phi;
                    int b = (theta + 1) * (phiDiv + 1) + phi;
                    int c = a + 1;
                    int d = b + 1;

                    mesh.TriangleIndices.Add(a);
                    mesh.TriangleIndices.Add(b);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(c);
                    mesh.TriangleIndices.Add(b);
                    mesh.TriangleIndices.Add(d);
                }
            }

            return mesh;
        }
    }
}
