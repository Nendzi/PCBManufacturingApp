using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using PCBManufacturing.Commands;
using PCBManufacturing.Models;
using PCBManufacturing.Services;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PCBManufacturing.ViewModels
{
	public class MainWindowViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

		private DropdownListWithLabelViewModel m_materialPreference;
		private DropdownListWithLabelViewModel m_solderMaskPreferences;
		private DropdownListWithLabelViewModel m_thicknessPreference;

		private ObservableCollection<OptionItemModel> m_allSolderMasks = new();
		private ObservableCollection<OptionItemModel> m_allThicknesses = new();
		private ObservableCollection<InventoryModel> m_allInventory = new();

		private string m_pricingIncrease;
		private string m_deliveryTimeExtending;
		private double m_priceDayQty;
		private string m_postalCode;

		private InventoryModel m_inventory;
		private List<double> m_widths;
		private List<double> m_lenghts;
		private List<int> m_layersQtys;

		private double m_width;
		private double m_length;
		private int m_layerQty;
		private Model3DCollection m_layerModels;

		private readonly IDialogService m_dialogService;

		private readonly Dictionary<string, List<string>> m_errors = new();

		#region Declarations
		public DropdownListWithLabelViewModel MaterialPreference
		{
			get => m_materialPreference;
			set { m_materialPreference = value; OnPropertyChanged(); }
		}

		public DropdownListWithLabelViewModel SolderMaskPreferences
		{
			get => m_solderMaskPreferences;
			set { m_solderMaskPreferences = value; OnPropertyChanged(); }
		}

		public DropdownListWithLabelViewModel ThicknessPreference
		{
			get => m_thicknessPreference;
			set { m_thicknessPreference = value; OnPropertyChanged(); }
		}

		public string PriceIncreasing
		{
			get => m_pricingIncrease;
			set { m_pricingIncrease = value; OnPropertyChanged(); }
		}

		public string DeliveryTimeExtended
		{
			get { return m_deliveryTimeExtending; }
			set { m_deliveryTimeExtending = value; OnPropertyChanged(); }
		}

		public double PriceDayQty
		{
			get => m_priceDayQty;
			set { m_priceDayQty = value; OnPropertyChanged(); }
		}

		public string PostalCode
		{
			get => m_postalCode;
			set
			{
				m_postalCode = value;
				OnPropertyChanged();
				ValidatePostalCode();

				OnPropertyChanged(nameof(IsPostalCodeValid));
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public bool IsPostalCodeValid => !HasErrors;

		public bool HasErrors => m_errors.Any();

		public InventoryModel Inventory
		{
			get => m_inventory;
			set { m_inventory = value; OnPropertyChanged(); }
		}

		public List<double> Widths
		{
			get => m_widths;
			set { m_widths = value; OnPropertyChanged(); }
		}

		public List<double> Lenghts
		{
			get => m_lenghts;
			set { m_lenghts = value; OnPropertyChanged(); }
		}

		public List<int> LayersQtys
		{
			get => m_layersQtys;
			set { m_layersQtys = value; OnPropertyChanged(); }
		}

		public double Width
		{
			get => m_width;
			set
			{
				m_width = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Purchase));
				RebuildLayerModels();
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public double Length
		{
			get => m_length;
			set
			{
				m_length = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Purchase));
				RebuildLayerModels();
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public int LayerQty
		{
			get => m_layerQty;
			set
			{
				m_layerQty = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Purchase));
				RebuildLayerModels();
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public string Purchase
		{
			get
			{
				if (CanConfirmRequest())
				{
					return $"{Width}mm x {Length}mm and {LayerQty} layer(s)";
				}
				return "Please make your purchase on Quote tab and set correct postal code";
			}
		}

		public Model3DCollection PCBLayersModel
		{
			get => m_layerModels;
			private set
			{
				m_layerModels = value;
				OnPropertyChanged();
			}
		}

		public ICommand ConfirmRequestCommand { get; }

		#endregion


		public MainWindowViewModel(IDialogService dialogService)
		{
			m_dialogService = dialogService;

			// Initialize each preference with different data. This should be in database and load through adequate service
			MaterialPreference = new DropdownListWithLabelViewModel(false)
			{
				Label = "Material:",
				Options = new ObservableCollection<OptionItemModel>
								{
										new OptionItemModel {Id = 1, Category = "Material", DisplayName = "FR-4", Value = "FR-4", DaysExtend = 0, PriceRaise =0 },
										new OptionItemModel {Id = 2, Category = "Material", DisplayName = "CEM-1", Value = "CEM-1", DaysExtend = 2, PriceRaise =40 },
										new OptionItemModel {Id = 3, Category = "Material", DisplayName = "Polyimide", Value = "Polyimide", DaysExtend = 4, PriceRaise =120 }
								}
			};

			SolderMaskPreferences = new DropdownListWithLabelViewModel(true)
			{
				Label = "Solder mask:",
				Options = new ObservableCollection<OptionItemModel>
								{
										new OptionItemModel {Id = 1, Category = "SolderMask", DisplayName = "Red", Value = "#FF0000", ColorValue = Brushes.Red, AvaiableMaterialId = 1, DaysExtend = 0, PriceRaise =0 },
										new OptionItemModel {Id = 2, Category = "SolderMask", DisplayName = "Blue", Value = "#0000FF", ColorValue = Brushes.Blue, AvaiableMaterialId = 1, DaysExtend = 2, PriceRaise =55 },
										new OptionItemModel {Id = 3, Category = "SolderMask", DisplayName = "Green", Value = "#00FF00", ColorValue = Brushes.Green, AvaiableMaterialId = 1, DaysExtend = 4, PriceRaise =82 },
										new OptionItemModel {Id = 4, Category = "SolderMask", DisplayName = "Yellow", Value = "#FFFF00", ColorValue = Brushes.Yellow, AvaiableMaterialId = 1, DaysExtend = 6, PriceRaise =96 },
										new OptionItemModel {Id = 5, Category = "SolderMask", DisplayName = "Red", Value = "#FF0000", ColorValue = Brushes.Red, AvaiableMaterialId = 2, DaysExtend = 0, PriceRaise =0 },
										new OptionItemModel {Id = 6, Category = "SolderMask", DisplayName = "Blue", Value = "#0000FF", ColorValue = Brushes.Blue, AvaiableMaterialId = 2, DaysExtend = 2, PriceRaise =55 },
										new OptionItemModel {Id = 7, Category = "SolderMask", DisplayName = "Green", Value = "#00FF00", ColorValue = Brushes.Green, AvaiableMaterialId = 3, DaysExtend = 4, PriceRaise =82 },
										new OptionItemModel {Id = 8, Category = "SolderMask", DisplayName = "Yellow", Value = "#FFFF00", ColorValue = Brushes.Yellow, AvaiableMaterialId = 2, DaysExtend = 6, PriceRaise =96 },
										new OptionItemModel {Id = 9, Category = "SolderMask", DisplayName = "Yellow", Value = "#FFFF00", ColorValue = Brushes.Yellow, AvaiableMaterialId = 3, DaysExtend = 6, PriceRaise =96 }
								}
			};

			ThicknessPreference = new DropdownListWithLabelViewModel(false)
			{
				Label = "Thickness:",
				Options = new ObservableCollection<OptionItemModel>
								{
										new OptionItemModel {Category = "Thickness", DisplayName = "Thin (1.2mm)", Value = 12, AvaiableMaskId = 1 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Medium (2.0mm)", Value = 20, AvaiableMaskId = 2},
										new OptionItemModel {Category = "Thickness", DisplayName = "Thick (3.0mm)", Value = 30, AvaiableMaskId = 2 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Extra Thick (4.0mm)", Value = 40, AvaiableMaskId = 2 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Thin (1.2mm)", Value = 12, AvaiableMaskId = 3 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Medium (2.0mm)", Value = 20, AvaiableMaskId = 3},
										new OptionItemModel {Category = "Thickness", DisplayName = "Thick (3.0mm)", Value = 30, AvaiableMaskId = 3 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Extra Thick (4.0mm)", Value = 40, AvaiableMaskId = 4 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Thin (1.2mm)", Value = 12, AvaiableMaskId = 4 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Medium (2.0mm)", Value = 20, AvaiableMaskId = 4},
										new OptionItemModel {Category = "Thickness", DisplayName = "Thick (3.0mm)", Value = 30, AvaiableMaskId = 4 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Extra Thick (4.0mm)", Value = 40, AvaiableMaskId = 5 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Thin (1.2mm)", Value = 12, AvaiableMaskId = 5 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Medium (2.0mm)", Value = 20, AvaiableMaskId = 6},
										new OptionItemModel {Category = "Thickness", DisplayName = "Thick (3.0mm)", Value = 30, AvaiableMaskId = 6 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Extra Thick (4.0mm)", Value = 40, AvaiableMaskId = 6 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Thin (1.2mm)", Value = 12, AvaiableMaskId = 7 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Medium (2.0mm)", Value = 20, AvaiableMaskId = 7},
										new OptionItemModel {Category = "Thickness", DisplayName = "Thick (3.0mm)", Value = 30, AvaiableMaskId = 8 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Extra Thick (4.0mm)", Value = 40, AvaiableMaskId = 8 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Thin (1.2mm)", Value = 12, AvaiableMaskId = 9 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Medium (2.0mm)", Value = 20, AvaiableMaskId = 9},
										new OptionItemModel {Category = "Thickness", DisplayName = "Thick (3.0mm)", Value = 30, AvaiableMaskId = 9 },
										new OptionItemModel {Category = "Thickness", DisplayName = "Extra Thick (4.0mm)", Value = 40, AvaiableMaskId = 9 }
								}
			};

			m_allSolderMasks = new ObservableCollection<OptionItemModel>(SolderMaskPreferences.Options);
			m_allThicknesses = new ObservableCollection<OptionItemModel>(ThicknessPreference.Options);

			MaterialPreference.PropertyChanged += MaterialPreference_PropertyChanged;
			SolderMaskPreferences.PropertyChanged += SolderMaskPreference_PropertyChanged;

			MaterialPreference.SelectedOption = MaterialPreference.Options.First();

			var AllInventory = new ObservableCollection<InventoryModel>()
			{
				new InventoryModel(){ Width = 20, Lenght = 30, LayerQty = 1},
				new InventoryModel(){ Width = 20, Lenght = 30, LayerQty = 2},
				new InventoryModel(){ Width = 20, Lenght = 30, LayerQty = 3},
				new InventoryModel(){ Width = 25, Lenght = 40, LayerQty = 1},
				new InventoryModel(){ Width = 25, Lenght = 40, LayerQty = 2},
				new InventoryModel(){ Width = 25, Lenght = 40, LayerQty = 3},
				new InventoryModel(){ Width = 25, Lenght = 40, LayerQty = 4},
				new InventoryModel(){ Width = 30, Lenght = 50, LayerQty = 1},
				new InventoryModel(){ Width = 30, Lenght = 50, LayerQty = 2},
				new InventoryModel(){ Width = 30, Lenght = 50, LayerQty = 3},
				new InventoryModel(){ Width = 30, Lenght = 50, LayerQty = 4}
			};

			Widths = new List<double>(AllInventory.Select(x => x.Width).Distinct().ToList());
			Lenghts = new List<double>(AllInventory.Select(x => x.Lenght).Distinct().ToList());
			LayersQtys = new List<int>(AllInventory.Select(x => x.LayerQty).Distinct().ToList());

			RebuildLayerModels();

			PostalCode = "00000";

			ConfirmRequestCommand = new BasicRelayCommand(ConfirmRequest, CanConfirmRequest);
		}


		private void MaterialPreference_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DropdownListWithLabelViewModel.SelectedOption))
			{
				UpdateSolderMasks();
				UpdatePriceAndTime(sender);
			}
		}

		private void SolderMaskPreference_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DropdownListWithLabelViewModel.SelectedOption))
			{
				UpdateThicknesses();
				UpdatePriceAndTime(sender);
			}
		}

		private void UpdateSolderMasks()
		{
			if (MaterialPreference.SelectedOption == null) return;

			long materialId = MaterialPreference.SelectedOption.Id;

			var filtered = m_allSolderMasks.Where(x => x.AvaiableMaterialId == materialId).ToList();

			SolderMaskPreferences.Options = new ObservableCollection<OptionItemModel>(filtered);
			SolderMaskPreferences.SelectedOption = SolderMaskPreferences.Options.FirstOrDefault();

			UpdateThicknesses();
		}

		private void UpdateThicknesses()
		{
			if (SolderMaskPreferences.SelectedOption == null) return;

			long maskId = SolderMaskPreferences.SelectedOption.Id;
			var filtered = m_allThicknesses.Where(x => x.AvaiableMaskId == maskId).ToList();

			ThicknessPreference.Options = new ObservableCollection<OptionItemModel>(filtered);
			ThicknessPreference.SelectedOption = ThicknessPreference.Options.FirstOrDefault();
		}

		private void UpdatePriceAndTime(object? sender)
		{
			if (sender != null && sender is DropdownListWithLabelViewModel viewModel)
			{
				decimal price = viewModel.SelectedOption?.PriceRaise ?? 0m;
				PriceIncreasing = $"Price raise by {price.ToString("F2")}";

				int days = viewModel.SelectedOption?.DaysExtend ?? 0;
				string plural = days == 1 ? "day" : "days";
				DeliveryTimeExtended = $"Delivery time will be extended for: {days} {plural}";

				PriceDayQty = (double)(price + days);
			}
		}

		private void RebuildLayerModels()
		{
			if (Width <= 0 || Length <= 0 || LayerQty <= 0)
			{
				PCBLayersModel = new Model3DCollection();
				return;
			}

			var models = new Model3DCollection();

			double scale = 0.08;

			double x = Width * scale / 2.0;
			double y = Length * scale / 2.0;
			double zStep = 0.15;

			for (int i = 0; i < LayerQty; i++)
			{
				double z = i * zStep;

				var brush = i % 2 == 0
						? Brushes.SteelBlue
						: Brushes.SaddleBrown;

				var material = new DiffuseMaterial(brush);

				var model = new GeometryModel3D
				{
					Geometry = new MeshGeometry3D
					{
						Positions = new Point3DCollection
								{
										new Point3D(-x, -y, z),
										new Point3D( x, -y, z),
										new Point3D( x,  y, z),
										new Point3D(-x,  y, z)
								},
						TriangleIndices = new Int32Collection { 0, 1, 2, 0, 2, 3 }
					},
					Material = material,
					BackMaterial = material
				};

				models.Add(model);
			}
			PCBLayersModel = models;
		}

		private void ConfirmRequest()
		{
			bool result = m_dialogService.Confirm($"You purchased PCB with following spcecification: {Purchase}. Please confirm your request.");
			if (result)
			{
				m_dialogService.ShowMessage("Thank you very much.");
			}
		}

		private bool CanConfirmRequest()
		{
			bool result = Width > 0 && Length > 0 && LayerQty > 0 && IsPostalCodeValid;
			return result;
		}

		#region Postal code validation
		private void ValidatePostalCode()
		{
			ClearErrors(nameof(PostalCode));

			if (PostalCode == null || string.IsNullOrWhiteSpace(PostalCode.ToString()))
			{
				AddError(nameof(PostalCode), "Postal code is required.");
				return;
			}

			string postalCode = PostalCode.ToString().Trim();

			if (!int.TryParse(postalCode, out int numericValue))
			{
				AddError(nameof(PostalCode), "Postal code must contain only digits.");
			}

			if (numericValue < 10000 || numericValue > 99999)
			{
				AddError(nameof(PostalCode), "Postal code must be exactly 5 digits and cannot start with 0.");
			}
		}

		private void ClearErrors(string propertyName)
		{
			if (m_errors.Remove(propertyName)) ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		private void AddError(string propertyName, string error)
		{
			if (!m_errors.ContainsKey(propertyName)) m_errors[propertyName] = new List<string>();

			m_errors[propertyName].Add(error);
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}

		public IEnumerable GetErrors(string? propertyName)
		{
			if (propertyName is null) return Enumerable.Empty<string>();

			return m_errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
		}

		#endregion

		protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

	}
}
