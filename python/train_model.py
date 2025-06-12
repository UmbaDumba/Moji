from ultralytics import YOLO
import cv2
import matplotlib.pyplot as plt

# 학습된 모델 로드
model = YOLO("runs/detect/eye_tracking5/weights/best.pt")

# 웹캠 또는 이미지 입력 (0: 기본 카메라)
cap = cv2.VideoCapture(0)

ret, frame = cap.read()
cap.release()

if not ret:
    print("❌ 카메라에서 프레임을 가져오지 못했습니다.")
    exit()

# YOLOv8 추론
results = model(frame)

# 결과 이미지 가져오기
annotated_frame = results[0].plot()

# 결과 저장
cv2.imwrite("output.jpg", annotated_frame)
print("✅ 결과 이미지 저장 완료: output.jpg")

# matplotlib로 화면에 띄우기 (imshow 없이 가능)
plt.imshow(cv2.cvtColor(annotated_frame, cv2.COLOR_BGR2RGB))
plt.title("YOLOv8 Inference Result")
plt.axis("off")
plt.show()
