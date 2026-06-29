const imageInput = document.getElementById('imageInput');
const previewSection = document.getElementById('previewSection');
const previewImage = document.getElementById('previewImage');
const applyFilterButton = document.getElementById('applyFilterButton');
let filterActive = false;

imageInput.addEventListener('change', handleImageSelection);

function handleImageSelection() {
  const file = imageInput.files?.[0];
  if (!file) {
    return;
  }

  const url = URL.createObjectURL(file);
  previewImage.src = url;
  previewImage.classList.remove('grayscale');
  filterActive = false;
  applyFilterButton.textContent = 'Aplicar filtro cinza';
  previewSection.classList.remove('hidden');
}

applyFilterButton.addEventListener('click', () => {
  if (!previewImage.src) return;
  filterActive = !filterActive;
  previewImage.classList.toggle('grayscale', filterActive);
  applyFilterButton.textContent = filterActive ? 'Remover filtro' : 'Aplicar filtro cinza';
});

const uploadBox = document.querySelector('.upload-box');
uploadBox.addEventListener('dragover', (event) => {
  event.preventDefault();
  uploadBox.classList.add('drag-over');
});

uploadBox.addEventListener('dragleave', () => {
  uploadBox.classList.remove('drag-over');
});

uploadBox.addEventListener('drop', (event) => {
  event.preventDefault();
  uploadBox.classList.remove('drag-over');
  const files = event.dataTransfer?.files;
  if (!files?.length) return;
  imageInput.files = files;
  handleImageSelection();
});
